using AutoMapper;
using ElAlProjectCore.DTOs.ResponseDTOs.AdminResponseDTOs;
using ElAlProjectCore.DTOs.ResponseDTOs.PassengerResonseDTOs;
using ElAlProjectCore.Repositories;
using ElAlProjectCore.Services;

namespace ElAlProjectService.Services
{
    public class PassengerService : IPassengerService
    {
        private readonly IPassengerRepository _passengerRepository;
        private readonly IMapper _mapper;

        public PassengerService(IPassengerRepository passengerRepository, IMapper mapper)
        {
            _passengerRepository = passengerRepository;
            _mapper = mapper;
        }

        public async Task<AdminResponse_PassengerDTO> GetPassengerById(int id, CancellationToken cancellationToken)
        {
            var passenger = await _passengerRepository.GetPassengerById(id, cancellationToken);

            if (passenger == null)
                throw new KeyNotFoundException($"Passenger with id {id} was not found.");

            return _mapper.Map<AdminResponse_PassengerDTO>(passenger);
        }

        public async Task<(IEnumerable<AdminResponse_PassengerDTO> Items, int TotalCount)> GetAllPassengers(int page, int pageSize, CancellationToken cancellationToken)
        {
            var (items, totalCount) = await _passengerRepository.GetAllPassengersPaged(page, pageSize, cancellationToken);

            return (_mapper.Map<IEnumerable<AdminResponse_PassengerDTO>>(items), totalCount);
        }

        public async Task<PassengerResponse_PassengerDTO> GetMyProfile(int passengerId, CancellationToken cancellationToken)
        {
            var passenger = await _passengerRepository.GetPassengerById(passengerId, cancellationToken);

            if (passenger == null)
                throw new KeyNotFoundException($"Passenger with id {passengerId} was not found.");

            return _mapper.Map<PassengerResponse_PassengerDTO>(passenger);
        }

        public async Task DeletePassenger(int id, CancellationToken cancellationToken)
        {
            if (!await _passengerRepository.DeletePassenger(id, cancellationToken))
                throw new KeyNotFoundException($"Passenger with id {id} was not found.");
        }
    }
}
