using ElAlProjectCore.DTOs.RequstDTOs.AdminRequests;
using ElAlProjectCore.DTOs.ResponseDTOs.AdminResponseDTOs;
using ElAlProjectCore.DTOs.ResponseDTOs.PassengerResonseDTOs;

namespace ElAlProjectCore.Services
{
    public interface IFlightService
    {
        Task<AdminResponse_FlightDTO> AddFlight(AdminRequest_FlightDTO flight, CancellationToken cancellationToken);

        Task<AdminResponse_FlightDTO> GetFlightById(int id, CancellationToken cancellationToken);

        Task<(IEnumerable<AdminResponse_FlightDTO> Items, int TotalCount)> GetAllFlights(int page, int pageSize, CancellationToken cancellationToken);

        Task<(IEnumerable<PassengerResponse_FlightDTO> Items, int TotalCount)> GetAvailableFlights(int page, int pageSize, CancellationToken cancellationToken);

        Task<AdminResponse_FlightDTO> UpdateFlight(int id, AdminRequest_FlightDTO flight, CancellationToken cancellationToken);

        Task DeleteFlight(int id, CancellationToken cancellationToken);
    }
}
