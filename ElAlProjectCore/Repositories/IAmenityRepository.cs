using ElAlProjectCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElAlProjectCore.Repositories
{
    public interface IAmenityRepository
    {
        Task<Amenity> AddAmenity(Amenity amenity, CancellationToken cancellationToken);

        Task<Amenity?> GetAmenityById(int id, CancellationToken cancellationToken);

        Task<List<Amenity>> GetAmenitiesByIds(List<int> ids, CancellationToken cancellationToken);

        Task<(IEnumerable<Amenity> Items, int TotalCount)> GetAllAmenitiesPaged(int page, int pageSize, CancellationToken cancellationToken);

        Task<Amenity?> UpdateAmenity(int id, Amenity amenity, CancellationToken cancellationToken);

        Task<bool> DeleteAmenity(int id, CancellationToken cancellationToken);
    }
}
