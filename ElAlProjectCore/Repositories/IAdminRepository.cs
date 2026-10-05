using ElAlProjectCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElAlProjectCore.Repositories
{
    public interface IAdminRepository
    {
        Task<Admin> AddAdmin(Admin admin, CancellationToken cancellationToken);

        Task<Admin?> GetAdminById(int id, CancellationToken cancellationToken);

        Task<(IEnumerable<Admin> Items, int TotalCount)> GetAllAdminsPaged(int page, int pageSize, CancellationToken cancellationToken);

        Task<Admin?> UpdateAdmin(int id, Admin admin, CancellationToken cancellationToken);

        Task<bool> DeleteAdmin(int id, CancellationToken cancellationToken);
         Task<Admin?> GetByEmailForLoginAsync(string email, CancellationToken cancellationToken);

    }
}
