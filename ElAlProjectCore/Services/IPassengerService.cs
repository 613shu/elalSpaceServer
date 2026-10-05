using ElAlProjectCore.DTOs.ResponseDTOs.AdminResponseDTOs;

namespace ElAlProjectCore.Services
{
    public interface IPassengerService
    {
        Task<AdminResponse_PassengerDTO> GetPassengerById(int id, CancellationToken cancellationToken);

        Task<(IEnumerable<AdminResponse_PassengerDTO> Items, int TotalCount)> GetAllPassengers(int page, int pageSize, CancellationToken cancellationToken);

        Task DeletePassenger(int id, CancellationToken cancellationToken);
    }
}
