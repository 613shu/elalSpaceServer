using AutoMapper;
using ElAlProjectCore.DTOs.ResponseDTOs.AdminResponseDTOs;
using ElAlProjectCore.Repositories;
using ElAlProjectCore.Services;

namespace ElAlProjectService.Services
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;
        private readonly IMapper _mapper;

        public AdminService(IAdminRepository adminRepository, IMapper mapper)
        {
            _adminRepository = adminRepository;
            _mapper = mapper;
        }

        public async Task<AdminResponse_AdminDTO> GetAdminById(int id, CancellationToken cancellationToken)
        {
            var admin = await _adminRepository.GetAdminById(id, cancellationToken);

            if (admin == null)
                throw new KeyNotFoundException($"Admin with id {id} was not found.");

            return _mapper.Map<AdminResponse_AdminDTO>(admin);
        }

        public async Task<(IEnumerable<AdminResponse_AdminDTO> Items, int TotalCount)> GetAllAdmins(int page, int pageSize, CancellationToken cancellationToken)
        {
            var (items, totalCount) = await _adminRepository.GetAllAdminsPaged(page, pageSize, cancellationToken);

            return (_mapper.Map<IEnumerable<AdminResponse_AdminDTO>>(items), totalCount);
        }

        public async Task DeleteAdmin(int id, CancellationToken cancellationToken)
        {
            if (!await _adminRepository.DeleteAdmin(id, cancellationToken))
                throw new KeyNotFoundException($"Admin with id {id} was not found.");
        }
    }
}
