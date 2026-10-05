using ElAlProjectCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElAlProjectCore.Repositories
{
    public interface IPassengerRepository
    {
        Task<Passenger> AddPassenger(Passenger passenger, CancellationToken cancellationToken);

        Task<Passenger?> GetPassengerById(int id, CancellationToken cancellationToken);

        Task<(IEnumerable<Passenger> Items, int TotalCount)> GetAllPassengersPaged(int page, int pageSize, CancellationToken cancellationToken);

        Task<Passenger?> UpdatePassenger(int id, Passenger passenger, CancellationToken cancellationToken);

        Task<bool> DeletePassenger(int id, CancellationToken cancellationToken);

        Task<Passenger?> GetByEmailForLoginAsync(string email, CancellationToken cancellationToken);

    }
}
