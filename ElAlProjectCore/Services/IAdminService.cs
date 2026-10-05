using ElAlProjectCore.DTOs.ResponseDTOs.AdminResponseDTOs;

namespace ElAlProjectCore.Services
{
    public interface IAdminService
    {
        Task<AdminResponse_AdminDTO> GetAdminById(int id, CancellationToken cancellationToken);

        Task<(IEnumerable<AdminResponse_AdminDTO> Items, int TotalCount)> GetAllAdmins(int page, int pageSize, CancellationToken cancellationToken);

        Task DeleteAdmin(int id, CancellationToken cancellationToken);
    }
}
