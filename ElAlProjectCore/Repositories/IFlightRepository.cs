using ElAlProjectCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElAlProjectCore.Repositories
{
    public interface IFlightRepository
    {
        Task<Flight> AddFlight(Flight flight, CancellationToken cancellationToken);

        Task<Flight?> GetFlightById(int id, CancellationToken cancellationToken);

        Task<Flight?> GetFlightForUpdate(int id, CancellationToken cancellationToken);

        void ClearTracking();

        Task<(IEnumerable<Flight> Items, int TotalCount)> GetAllFlightsPaged(int page, int pageSize, CancellationToken cancellationToken);

        Task<(IEnumerable<Flight> Items, int TotalCount)> GetAvailableFlightsPaged(DateTime departsAfter, int page, int pageSize, CancellationToken cancellationToken);

        Task<Flight?> UpdateFlight(int id, Flight flight, CancellationToken cancellationToken);

        Task<bool> DeleteFlight(int id, CancellationToken cancellationToken);
    }
}
