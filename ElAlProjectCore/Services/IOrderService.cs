using ElAlProjectCore.DTOs.RequstDTOs.PassengerRequest;
using ElAlProjectCore.DTOs.ResponseDTOs.AdminResponseDTOs;
using ElAlProjectCore.DTOs.ResponseDTOs.PassengerResonseDTOs;

namespace ElAlProjectCore.Services
{
    public interface IOrderService
    {
        Task<PassengerResponse_OrderDTO> AddOrder(int passengerId, PassengerRequest_OrderDTO order, CancellationToken cancellationToken);

        Task<AdminResponse_OrderDTO> GetOrderById(int id, CancellationToken cancellationToken);

        Task<(IEnumerable<PassengerResponse_OrderDTO> Items, int TotalCount)> GetPassengerOrders(int passengerId, int page, int pageSize, CancellationToken cancellationToken);

        Task<(IEnumerable<AdminResponse_OrderDTO> Items, int TotalCount)> GetAllOrders(int page, int pageSize, CancellationToken cancellationToken);

        Task DeleteOrder(int id, int passengerId, bool isAdmin, CancellationToken cancellationToken);
    }
}
