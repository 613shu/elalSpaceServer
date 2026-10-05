using ElAlProjectCore.DTOs.RequstDTOs.AdminRequests;
using ElAlProjectCore.DTOs.ResponseDTOs.AdminResponseDTOs;

namespace ElAlProjectCore.Services
{
    public interface IAmenityService
    {
        Task<AdminResponse_AmenityDTO> AddAmenity(AdminRequest_AmenityDTO amenity, CancellationToken cancellationToken);

        Task<AdminResponse_AmenityDTO> GetAmenityById(int id, CancellationToken cancellationToken);

        Task<(IEnumerable<AdminResponse_AmenityDTO> Items, int TotalCount)> GetAllAmenities(int page, int pageSize, CancellationToken cancellationToken);

        Task<AdminResponse_AmenityDTO> UpdateAmenity(int id, AdminRequest_AmenityDTO amenity, CancellationToken cancellationToken);

        Task DeleteAmenity(int id, CancellationToken cancellationToken);
    }
}
