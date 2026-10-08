using AutoMapper;
using ElAlProjectCore.DTOs.RequstDTOs.AdminRequests;
using ElAlProjectCore.DTOs.ResponseDTOs.AdminResponseDTOs;
using ElAlProjectCore.DTOs.ResponseDTOs.PassengerResonseDTOs;
using ElAlProjectCore.Models;
using ElAlProjectCore.Repositories;
using ElAlProjectCore.Services;

namespace ElAlProjectService.Services
{
    public class FlightService : IFlightService
    {
        private readonly IFlightRepository _flightRepository;
        private readonly IAmenityRepository _amenityRepository;
        private readonly IMapper _mapper;

        public FlightService(IFlightRepository flightRepository, IAmenityRepository amenityRepository, IMapper mapper)
        {
            _flightRepository = flightRepository;
            _amenityRepository = amenityRepository;
            _mapper = mapper;
        }

        public async Task<AdminResponse_FlightDTO> AddFlight(AdminRequest_FlightDTO flight, CancellationToken cancellationToken)
        {
            var flightMap = await MapFlight(flight, cancellationToken);
            flightMap.AvailableSeats = flight.NumOfSeats;

            var res = await _flightRepository.AddFlight(flightMap, cancellationToken);

            return _mapper.Map<AdminResponse_FlightDTO>(res);
        }

        public async Task<AdminResponse_FlightDTO> GetFlightById(int id, CancellationToken cancellationToken)
        {
            var flight = await _flightRepository.GetFlightById(id, cancellationToken);

            if (flight == null)
                throw new KeyNotFoundException($"Flight with id {id} was not found.");

            return _mapper.Map<AdminResponse_FlightDTO>(flight);
        }

        public async Task<(IEnumerable<AdminResponse_FlightDTO> Items, int TotalCount)> GetAllFlights(int page, int pageSize, CancellationToken cancellationToken)
        {
            var (items, totalCount) = await _flightRepository.GetAllFlightsPaged(page, pageSize, cancellationToken);

            return (_mapper.Map<IEnumerable<AdminResponse_FlightDTO>>(items), totalCount);
        }

        public async Task<(IEnumerable<PassengerResponse_FlightDTO> Items, int TotalCount)> GetAvailableFlights(int page, int pageSize, CancellationToken cancellationToken)
        {
            var (items, totalCount) = await _flightRepository.GetAvailableFlightsPaged(DateTime.UtcNow, page, pageSize, cancellationToken);

            return (_mapper.Map<IEnumerable<PassengerResponse_FlightDTO>>(items), totalCount);
        }

        public async Task<AdminResponse_FlightDTO> UpdateFlight(int id, AdminRequest_FlightDTO flight, CancellationToken cancellationToken)
        {
            var flightMap = await MapFlight(flight, cancellationToken);

            var updated = await _flightRepository.UpdateFlight(id, flightMap, cancellationToken);

            if (updated == null)
                throw new KeyNotFoundException($"Flight with id {id} was not found.");

            return _mapper.Map<AdminResponse_FlightDTO>(updated);
        }

        public async Task DeleteFlight(int id, CancellationToken cancellationToken)
        {
            if (!await _flightRepository.DeleteFlight(id, cancellationToken))
                throw new KeyNotFoundException($"Flight with id {id} was not found.");
        }

        private async Task<Flight> MapFlight(AdminRequest_FlightDTO flight, CancellationToken cancellationToken)
        {
            if (flight.ArrivalTime <= flight.DepartureTime)
                throw new ArgumentException("Arrival time must be after departure time.");

            var amenities = await _amenityRepository.GetAmenitiesByIds(flight.AmenityIds, cancellationToken);

            if (amenities.Count != flight.AmenityIds.Distinct().Count())
                throw new KeyNotFoundException("One or more amenities were not found.");

            var flightMap = _mapper.Map<Flight>(flight);
            flightMap.Amenities = amenities;

            return flightMap;
        }
    }
}
