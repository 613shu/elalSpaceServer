using System.ComponentModel.DataAnnotations;

namespace ElAlProjectCore.DTOs.RequstDTOs.PassengerRequest
{
    public class PassengerRequest_OrderDTO
    {
        [Range(1, int.MaxValue)]
        public int FlightId { get; set; }
    }
}
