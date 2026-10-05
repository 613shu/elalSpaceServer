using ElAlProjectCore.Models;
using ElAlProjectCore.Repositories;
using ElAlProjectData.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElAlProjectData.Repositories
{
    public class AmenityRepository : IAmenityRepository
    {
        private readonly DataContext _dataContext;

        public AmenityRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }


        public async Task<Amenity> AddAmenity(Amenity amenity, CancellationToken cancellationToken)
        {
            await _dataContext.Amenities.AddAsync(amenity, cancellationToken);
            await _dataContext.SaveChangesAsync(cancellationToken);

            return amenity;
        }


        public async Task<Amenity?> GetAmenityById(int id, CancellationToken cancellationToken)
        {
            return await _dataContext.Amenities.AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        }


        public async Task<List<Amenity>> GetAmenitiesByIds(List<int> ids, CancellationToken cancellationToken)
        {
            return await _dataContext.Amenities
                .Where(a => ids.Contains(a.Id))
                .ToListAsync(cancellationToken);
        }


        //pagination
        public async Task<(IEnumerable<Amenity> Items, int TotalCount)> GetAllAmenitiesPaged(int page, int pageSize, CancellationToken cancellationToken)
        {
            var query = _dataContext.Amenities.AsNoTracking()
                .OrderBy(a => a.Name)
                .ThenBy(a => a.Id);

            return await query.ToPagedAsync(page, pageSize, cancellationToken);
        }


        // Returns null when the amenity does not exist
        public async Task<Amenity?> UpdateAmenity(int id, Amenity amenity, CancellationToken cancellationToken)
        {
            var existing = await _dataContext.Amenities
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

            if (existing == null)
                return null;

            existing.Name = amenity.Name;

            await _dataContext.SaveChangesAsync(cancellationToken);

            return existing;
        }


        // Real delete: the amenity is removed, together with its links to flights.
        // Returns false when the amenity does not exist
        public async Task<bool> DeleteAmenity(int id, CancellationToken cancellationToken)
        {
            var existing = await _dataContext.Amenities
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

            if (existing == null)
                return false;

            _dataContext.Amenities.Remove(existing);

            await _dataContext.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
