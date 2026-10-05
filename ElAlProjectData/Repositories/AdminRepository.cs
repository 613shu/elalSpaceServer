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
    public class AdminRepository : IAdminRepository
    {
        private readonly DataContext _dataContext;

        public AdminRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }


        public async Task<Admin> AddAdmin(Admin admin, CancellationToken cancellationToken)
        {
            await _dataContext.Admins.AddAsync(admin, cancellationToken);
            await _dataContext.SaveChangesAsync(cancellationToken);

            return admin;
        }


        public async Task<Admin?> GetAdminById(int id, CancellationToken cancellationToken)
        {
            return await _dataContext.Admins.AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id && a.IsActive, cancellationToken);
        }


        public async Task<(IEnumerable<Admin> Items, int TotalCount)> GetAllAdminsPaged(int page, int pageSize, CancellationToken cancellationToken)
        {
            var query = _dataContext.Admins.AsNoTracking()
                .Where(a => a.IsActive)
                .OrderBy(a => a.Id);

            return await query.ToPagedAsync(page, pageSize, cancellationToken);
        }


        public async Task<Admin?> UpdateAdmin(int id, Admin admin, CancellationToken cancellationToken)
        {
            var existing = await _dataContext.Admins
                .FirstOrDefaultAsync(a => a.Id == id && a.IsActive, cancellationToken);

            if (existing == null)
                return null;

            existing.Name = admin.Name;
            existing.Password = admin.Password;

            await _dataContext.SaveChangesAsync(cancellationToken);

            return existing;
        }


      
        public async Task<bool> DeleteAdmin(int id, CancellationToken cancellationToken)
        {
            var existing = await _dataContext.Admins
                .FirstOrDefaultAsync(a => a.Id == id && a.IsActive, cancellationToken);

            if (existing == null)
                return false;

            existing.IsActive = false;

            await _dataContext.SaveChangesAsync(cancellationToken);

            return true;
        }



        public async Task<Admin?> GetByEmailForLoginAsync(string email, CancellationToken cancellationToken)
        {
            return await _dataContext.Admins
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Email == email&&a.IsActive==true, cancellationToken);
        }

    }
}
