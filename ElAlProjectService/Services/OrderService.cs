using AutoMapper;
using ElAlProjectCore.DTOs.RequstDTOs.PassengerRequest;
using ElAlProjectCore.DTOs.ResponseDTOs.AdminResponseDTOs;
using ElAlProjectCore.DTOs.ResponseDTOs.PassengerResonseDTOs;
using ElAlProjectCore.Enums;
using ElAlProjectCore.Models;
using ElAlProjectCore.Repositories;
using ElAlProjectCore.Services;
using Microsoft.EntityFrameworkCore;

namespace ElAlProjectService.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IFlightRepository _flightRepository;
        private readonly IMapper _mapper;

        public OrderService(IOrderRepository orderRepository, IFlightRepository flightRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _flightRepository = flightRepository;
            _mapper = mapper;
        }

        //שניים שמתחרים 
        public async Task<PassengerResponse_OrderDTO> AddOrder(int passengerId, PassengerRequest_OrderDTO order, CancellationToken cancellationToken)
        {
            var flight = await _flightRepository.GetFlightForUpdate(order.FlightId, cancellationToken);

            if (flight == null)
                throw new KeyNotFoundException($"Flight with id {order.FlightId} was not found.");
            //יכולות הזמנות להתפס מקסימום כמספר המקומות
            while (flight.AvailableSeats > 0)
            {
                if (flight.FlightStatus != FlightStatus.Scheduled || flight.DepartureTime <= DateTime.UtcNow)
                    throw new InvalidOperationException("This flight is not open for booking.");

                if (await _orderRepository.PassengerHasOrder(passengerId, flight.Id, cancellationToken))
                    throw new InvalidOperationException("You already have an order for this flight.");

                flight.AvailableSeats--;

                var newOrder = new Order
                {
                    FlightId = flight.Id,
                    PassengerId = passengerId,
                    OrderDateTime = DateTime.UtcNow,
                    Status = OrderStatus.Confirmed
                };

                try
                {
                    await _orderRepository.AddOrder(newOrder, cancellationToken);
                    return _mapper.Map<PassengerResponse_OrderDTO>(newOrder);
                }
                catch (DbUpdateConcurrencyException)
                {
                    _flightRepository.ClearTracking();
                    flight = await _flightRepository.GetFlightForUpdate(order.FlightId, cancellationToken);

                    if (flight == null)
                        throw new KeyNotFoundException($"Flight with id {order.FlightId} was not found.");
                }
            }

            throw new InvalidOperationException("No seats available on this flight.");
        }

        public async Task<AdminResponse_OrderDTO> GetOrderById(int id, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetOrderById(id, cancellationToken);

            if (order == null)
                throw new KeyNotFoundException($"Order with id {id} was not found.");

            return _mapper.Map<AdminResponse_OrderDTO>(order);
        }

        public async Task<(IEnumerable<PassengerResponse_OrderDTO> Items, int TotalCount)> GetPassengerOrders(int passengerId, int page, int pageSize, CancellationToken cancellationToken)
        {
            var (items, totalCount) = await _orderRepository.GetPassengerOrdersPaged(passengerId, page, pageSize, cancellationToken);

            return (_mapper.Map<IEnumerable<PassengerResponse_OrderDTO>>(items), totalCount);
        }

        public async Task<(IEnumerable<AdminResponse_OrderDTO> Items, int TotalCount)> GetAllOrders(int page, int pageSize, CancellationToken cancellationToken)
        {
            var (items, totalCount) = await _orderRepository.GetAllOrdersPaged(page, pageSize, cancellationToken);

            return (_mapper.Map<IEnumerable<AdminResponse_OrderDTO>>(items), totalCount);
        }

        public async Task DeleteOrder(int id, int passengerId, bool isAdmin, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetOrderForUpdate(id, cancellationToken);

            if (order == null)
                throw new KeyNotFoundException($"Order with id {id} was not found.");

            if (!isAdmin && order.PassengerId != passengerId)
                throw new UnauthorizedAccessException("You can cancel only your own orders.");

            if (order.Status == OrderStatus.Cancelled)
                throw new InvalidOperationException("This order is already cancelled.");

            if (order.Flight.DepartureTime <= DateTime.UtcNow)
                throw new InvalidOperationException("An order cannot be cancelled after the flight has departed.");

            order.Flight.AvailableSeats++;

            await _orderRepository.DeleteOrder(id, cancellationToken);
        }
    }
}
