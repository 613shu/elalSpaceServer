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
    public class PassengerRepository : IPassengerRepository
    {
        private readonly DataContext _dataContext;

        public PassengerRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }


        public async Task<Passenger> AddPassenger(Passenger passenger, CancellationToken cancellationToken)
        {
            await _dataContext.Passengers.AddAsync(passenger, cancellationToken);
            await _dataContext.SaveChangesAsync(cancellationToken);

            return passenger;
        }


        public async Task<Passenger?> GetPassengerById(int id, CancellationToken cancellationToken)
        {
            return await _dataContext.Passengers.AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive, cancellationToken);
        }


        public async Task<(IEnumerable<Passenger> Items, int TotalCount)> GetAllPassengersPaged(int page, int pageSize, CancellationToken cancellationToken)
        {
            var query = _dataContext.Passengers.AsNoTracking()
                .Where(p => p.IsActive)
                .OrderBy(p => p.Id);

            return await query.ToPagedAsync(page, pageSize, cancellationToken);
        }


        public async Task<Passenger?> UpdatePassenger(int id, Passenger passenger, CancellationToken cancellationToken)
        {
            var existing = await _dataContext.Passengers
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive, cancellationToken);

            if (existing == null)
                return null;

            existing.Name = passenger.Name;
            existing.Passward = passenger.Passward;

            await _dataContext.SaveChangesAsync(cancellationToken);

            return existing;
        }


        public async Task<bool> DeletePassenger(int id, CancellationToken cancellationToken)
        {
            var existing = await _dataContext.Passengers
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive, cancellationToken);

            if (existing == null)
                return false;

            existing.IsActive = false;

            await _dataContext.SaveChangesAsync(cancellationToken);

            return true;
        }


        public async Task<Passenger?> GetByEmailForLoginAsync(string email, CancellationToken cancellationToken)
        {
            return await _dataContext.Passengers
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Email == email&&a.IsActive==true, cancellationToken);
        }
    }
}
