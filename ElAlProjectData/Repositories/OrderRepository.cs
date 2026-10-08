using ElAlProjectCore.Enums;
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
    public class OrderRepository : IOrderRepository
    {
        private readonly DataContext _dataContext;
        public OrderRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<Order> AddOrder(Order order, CancellationToken cancellationToken)
        {
            await _dataContext.Orders.AddAsync(order, cancellationToken);
            await _dataContext.SaveChangesAsync(cancellationToken);

            return order;
        }



        public async Task<Order?> GetOrderById(int id, CancellationToken cancellationToken)
        {
            return await _dataContext.Orders.AsNoTracking()
                .Include(o => o.Flight)
                    .ThenInclude(f => f.Amenities)
                .Include(o => o.Passenger)
                .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
        }





        public async Task<Order?> GetOrderForUpdate(int id, CancellationToken cancellationToken)
        {
            return await _dataContext.Orders
                .Include(o => o.Flight)
                .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
        }



        public async Task<bool> PassengerHasOrder(int passengerId, int flightId, CancellationToken cancellationToken)
        {
            return await _dataContext.Orders
                .AnyAsync(o => o.PassengerId == passengerId && o.FlightId == flightId && o.Status == OrderStatus.Confirmed, cancellationToken);
        }



        //pagination
        public async Task<(IEnumerable<Order> Items, int TotalCount)> GetPassengerOrdersPaged(int passengerId, int page, int pageSize, CancellationToken cancellationToken)
        {
            var query = _dataContext.Orders.AsNoTracking()
                .Include(o => o.Flight)
                    .ThenInclude(f => f.Amenities)
                .Where(o => o.Passenger.Id == passengerId)
                .OrderByDescending(o => o.OrderDateTime)
                .ThenByDescending(o => o.Id);

            return await query.ToPagedAsync(page, pageSize, cancellationToken);
        }




    


        public async Task<(IEnumerable<Order> Items, int TotalCount)> GetAllOrdersPaged(int page, int pageSize, CancellationToken cancellationToken)
        {
            IQueryable<Order> query = _dataContext.Orders.AsNoTracking()
                .Include(o => o.Flight)
                    .ThenInclude(f => f.Amenities)
                .Include(o => o.Passenger);


            query = query
                .OrderByDescending(o => o.OrderDateTime)
                .ThenByDescending(o => o.Id);

            return await query.ToPagedAsync(page, pageSize, cancellationToken);
        }



        // Soft delete: the order stays in the database with status Cancelled.
        // Returns false when the order does not exist or is already cancelled
        public async Task<bool> DeleteOrder(int id, CancellationToken cancellationToken)
        {
            var existing = await _dataContext.Orders
                .FirstOrDefaultAsync(o => o.Id == id && o.Status != OrderStatus.Cancelled, cancellationToken);

            if (existing == null)
                return false;

            existing.Status = OrderStatus.Cancelled;
            existing.CancelledAt = DateTime.UtcNow;

            await _dataContext.SaveChangesAsync(cancellationToken);

            return true;
        }

    }
}
