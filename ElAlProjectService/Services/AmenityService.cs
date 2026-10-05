using AutoMapper;
using ElAlProjectCore.DTOs.RequstDTOs.AdminRequests;
using ElAlProjectCore.DTOs.ResponseDTOs.AdminResponseDTOs;
using ElAlProjectCore.Models;
using ElAlProjectCore.Repositories;
using ElAlProjectCore.Services;

namespace ElAlProjectService.Services
{
    public class AmenityService : IAmenityService
    {
        private readonly IAmenityRepository _amenityRepository;
        private readonly IMapper _mapper;

        public AmenityService(IAmenityRepository amenityRepository, IMapper mapper)
        {
            _amenityRepository = amenityRepository;
            _mapper = mapper;
        }

        public async Task<AdminResponse_AmenityDTO> AddAmenity(AdminRequest_AmenityDTO amenity, CancellationToken cancellationToken)
        {
            var amenityMap = _mapper.Map<Amenity>(amenity);

            var res = await _amenityRepository.AddAmenity(amenityMap, cancellationToken);

            return _mapper.Map<AdminResponse_AmenityDTO>(res);
        }

        public async Task<AdminResponse_AmenityDTO> GetAmenityById(int id, CancellationToken cancellationToken)
        {
            var amenity = await _amenityRepository.GetAmenityById(id, cancellationToken);

            if (amenity == null)
                throw new KeyNotFoundException($"Amenity with id {id} was not found.");

            return _mapper.Map<AdminResponse_AmenityDTO>(amenity);
        }

        public async Task<(IEnumerable<AdminResponse_AmenityDTO> Items, int TotalCount)> GetAllAmenities(int page, int pageSize, CancellationToken cancellationToken)
        {
            var (items, totalCount) = await _amenityRepository.GetAllAmenitiesPaged(page, pageSize, cancellationToken);

            return (_mapper.Map<IEnumerable<AdminResponse_AmenityDTO>>(items), totalCount);
        }

        public async Task<AdminResponse_AmenityDTO> UpdateAmenity(int id, AdminRequest_AmenityDTO amenity, CancellationToken cancellationToken)
        {
            var amenityMap = _mapper.Map<Amenity>(amenity);

            var updated = await _amenityRepository.UpdateAmenity(id, amenityMap, cancellationToken);

            if (updated == null)
                throw new KeyNotFoundException($"Amenity with id {id} was not found.");

            return _mapper.Map<AdminResponse_AmenityDTO>(updated);
        }

        public async Task DeleteAmenity(int id, CancellationToken cancellationToken)
        {
            if (!await _amenityRepository.DeleteAmenity(id, cancellationToken))
                throw new KeyNotFoundException($"Amenity with id {id} was not found.");
        }
    }
}
