using AutoMapper;
using ElAlProjectCore.DTOs.RequstDTOs.PassengerRequest;
using ElAlProjectCore.Enums;
using ElAlProjectCore.Models;
using ElAlProjectCore.Repositories;
using ElAlProjectService.Mapping;
using ElAlProjectService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace ElAlProjectTests.Services
{
    public class OrderServiceTests
    {
        private const int PassengerId = 7;
        private const int FlightId = 1;
        private const int OrderId = 5;

        private readonly Mock<IOrderRepository> _orderRepository = new();
        private readonly Mock<IFlightRepository> _flightRepository = new();
        private readonly OrderService _service;
        private readonly PassengerRequest_OrderDTO _request = new() { FlightId = FlightId };

        public OrderServiceTests()
        {
            var mapper = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>(), NullLoggerFactory.Instance).CreateMapper();

            _orderRepository
                .Setup(r => r.AddOrder(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Order order, CancellationToken _) => order);

            _service = new OrderService(_orderRepository.Object, _flightRepository.Object, mapper);
        }

        private static Flight CreateFlight(int availableSeats)
        {
            return new Flight
            {
                Id = FlightId,
                FlightNumber = "LY001",
                DepartureAirport = "TLV",
                ArrivalAirport = "JFK",
                DepartureTime = DateTime.UtcNow.AddDays(1),
                ArrivalTime = DateTime.UtcNow.AddDays(1).AddHours(11),
                NumOfSeats = 100,
                AvailableSeats = availableSeats,
                FlightStatus = FlightStatus.Scheduled
            };
        }

        private static Order CreateOrder(Flight flight)
        {
            return new Order
            {
                Id = OrderId,
                FlightId = flight.Id,
                Flight = flight,
                PassengerId = PassengerId,
                OrderDateTime = DateTime.UtcNow,
                Status = OrderStatus.Confirmed
            };
        }

        private void SetupFlight(Flight? flight)
        {
            _flightRepository
                .Setup(r => r.GetFlightForUpdate(FlightId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(flight);
        }

        private void SetupOrder(Order? order)
        {
            _orderRepository
                .Setup(r => r.GetOrderForUpdate(OrderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);
        }

        private void VerifyOrderSaved(Times times)
        {
            _orderRepository.Verify(r => r.AddOrder(It.IsAny<Order>(), It.IsAny<CancellationToken>()), times);
        }

        [Fact]
        public async Task AddOrder_WhenSeatAvailable_SavesOrderAndTakesSeat()
        {
            var flight = CreateFlight(1);
            SetupFlight(flight);

            var result = await _service.AddOrder(PassengerId, _request, CancellationToken.None);

            Assert.Equal("Confirmed", result.Status);
            Assert.Equal(0, flight.AvailableSeats);
            _orderRepository.Verify(r => r.AddOrder(
                It.Is<Order>(o => o.FlightId == FlightId && o.PassengerId == PassengerId && o.Status == OrderStatus.Confirmed),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AddOrder_WhenNoSeatsLeft_ThrowsAndDoesNotSave()
        {
            var flight = CreateFlight(0);
            SetupFlight(flight);

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.AddOrder(PassengerId, _request, CancellationToken.None));

            Assert.Equal("No seats available on this flight.", exception.Message);
            Assert.Equal(0, flight.AvailableSeats);
            VerifyOrderSaved(Times.Never());
        }

        [Fact]
        public async Task AddOrder_WhenFlightNotFound_ThrowsKeyNotFound()
        {
            SetupFlight(null);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _service.AddOrder(PassengerId, _request, CancellationToken.None));

            VerifyOrderSaved(Times.Never());
        }

        [Theory]
        [InlineData(FlightStatus.Cancelled)]
        [InlineData(FlightStatus.Completed)]
        public async Task AddOrder_WhenFlightNotScheduled_ThrowsAndKeepsSeat(FlightStatus status)
        {
            var flight = CreateFlight(5);
            flight.FlightStatus = status;
            SetupFlight(flight);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.AddOrder(PassengerId, _request, CancellationToken.None));

            Assert.Equal(5, flight.AvailableSeats);
            VerifyOrderSaved(Times.Never());
        }

        [Fact]
        public async Task AddOrder_WhenFlightAlreadyDeparted_ThrowsAndKeepsSeat()
        {
            var flight = CreateFlight(5);
            flight.DepartureTime = DateTime.UtcNow.AddMinutes(-1);
            SetupFlight(flight);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.AddOrder(PassengerId, _request, CancellationToken.None));

            Assert.Equal(5, flight.AvailableSeats);
            VerifyOrderSaved(Times.Never());
        }

        [Fact]
        public async Task AddOrder_WhenPassengerAlreadyHasOrder_ThrowsAndKeepsSeat()
        {
            var flight = CreateFlight(5);
            SetupFlight(flight);
            _orderRepository
                .Setup(r => r.PassengerHasOrder(PassengerId, FlightId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.AddOrder(PassengerId, _request, CancellationToken.None));

            Assert.Equal(5, flight.AvailableSeats);
            VerifyOrderSaved(Times.Never());
        }

        [Fact]
        public async Task AddOrder_WhenSaveConflictsAndSeatStillLeft_RetriesAndSucceeds()
        {
            var staleFlight = CreateFlight(2);
            var freshFlight = CreateFlight(1);
            _flightRepository
                .SetupSequence(r => r.GetFlightForUpdate(FlightId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(staleFlight)
                .ReturnsAsync(freshFlight);
            _orderRepository
                .SetupSequence(r => r.AddOrder(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new DbUpdateConcurrencyException("conflict"))
                .ReturnsAsync(new Order());

            var result = await _service.AddOrder(PassengerId, _request, CancellationToken.None);

            Assert.Equal("Confirmed", result.Status);
            Assert.Equal(0, freshFlight.AvailableSeats);
            _flightRepository.Verify(r => r.ClearTracking(), Times.Once);
            _flightRepository.Verify(r => r.GetFlightForUpdate(FlightId, It.IsAny<CancellationToken>()), Times.Exactly(2));
            VerifyOrderSaved(Times.Exactly(2));
        }

        [Fact]
        public async Task AddOrder_WhenSaveConflictsAndLastSeatWasTaken_Throws()
        {
            var staleFlight = CreateFlight(1);
            var freshFlight = CreateFlight(0);
            _flightRepository
                .SetupSequence(r => r.GetFlightForUpdate(FlightId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(staleFlight)
                .ReturnsAsync(freshFlight);
            _orderRepository
                .Setup(r => r.AddOrder(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new DbUpdateConcurrencyException("conflict"));

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.AddOrder(PassengerId, _request, CancellationToken.None));

            Assert.Equal("No seats available on this flight.", exception.Message);
            _flightRepository.Verify(r => r.ClearTracking(), Times.Once);
            VerifyOrderSaved(Times.Once());
        }

        [Fact]
        public async Task DeleteOrder_WhenOwnerCancels_ReturnsSeatAndCancels()
        {
            var flight = CreateFlight(0);
            SetupOrder(CreateOrder(flight));

            await _service.DeleteOrder(OrderId, PassengerId, false, CancellationToken.None);

            Assert.Equal(1, flight.AvailableSeats);
            _orderRepository.Verify(r => r.DeleteOrder(OrderId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteOrder_WhenAnotherPassengerCancels_ThrowsAndKeepsSeatTaken()
        {
            var flight = CreateFlight(0);
            SetupOrder(CreateOrder(flight));

            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _service.DeleteOrder(OrderId, PassengerId + 1, false, CancellationToken.None));

            Assert.Equal(0, flight.AvailableSeats);
            _orderRepository.Verify(r => r.DeleteOrder(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DeleteOrder_WhenAdminCancelsOrderOfPassenger_ReturnsSeatAndCancels()
        {
            var flight = CreateFlight(0);
            SetupOrder(CreateOrder(flight));

            await _service.DeleteOrder(OrderId, PassengerId + 1, true, CancellationToken.None);

            Assert.Equal(1, flight.AvailableSeats);
            _orderRepository.Verify(r => r.DeleteOrder(OrderId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteOrder_WhenAlreadyCancelled_ThrowsAndDoesNotReturnSeatAgain()
        {
            var flight = CreateFlight(1);
            var order = CreateOrder(flight);
            order.Status = OrderStatus.Cancelled;
            SetupOrder(order);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.DeleteOrder(OrderId, PassengerId, false, CancellationToken.None));

            Assert.Equal(1, flight.AvailableSeats);
            _orderRepository.Verify(r => r.DeleteOrder(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DeleteOrder_WhenOrderNotFound_ThrowsKeyNotFound()
        {
            SetupOrder(null);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _service.DeleteOrder(OrderId, PassengerId, false, CancellationToken.None));
        }
    }
}
