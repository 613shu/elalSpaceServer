using ElAlProjectCore.DTOs.ResponseDTOs.AdminResponseDTOs;
using ElAlProjectCore.DTOs.ResponseDTOs.PassengerResonseDTOs;

namespace ElAlProjectCore.Services
{
    public interface IPassengerService
    {
        Task<AdminResponse_PassengerDTO> GetPassengerById(int id, CancellationToken cancellationToken);

        Task<(IEnumerable<AdminResponse_PassengerDTO> Items, int TotalCount)> GetAllPassengers(int page, int pageSize, CancellationToken cancellationToken);

        Task<PassengerResponse_PassengerDTO> GetMyProfile(int passengerId, CancellationToken cancellationToken);

        Task DeletePassenger(int id, CancellationToken cancellationToken);
    }
}
