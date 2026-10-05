using ElAlProjectCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElAlProjectCore.Repositories
{
    public interface IOrderRepository
    {
        Task<Order> AddOrder(Order order, CancellationToken cancellationToken);

        Task<Order?> GetOrderById(int id, CancellationToken cancellationToken);

        Task<Order?> GetOrderForUpdate(int id, CancellationToken cancellationToken);

        Task<bool> PassengerHasOrder(int passengerId, int flightId, CancellationToken cancellationToken);

        Task<(IEnumerable<Order> Items, int TotalCount)> GetPassengerOrdersPaged(int passengerId, int page, int pageSize, CancellationToken cancellationToken);

        Task<(IEnumerable<Order> Items, int TotalCount)> GetAllOrdersPaged(int page, int pageSize, CancellationToken cancellationToken);

        Task<bool> DeleteOrder(int id, CancellationToken cancellationToken);

        Task CancelFlightOrders(int flightId, CancellationToken cancellationToken);
    }
}
