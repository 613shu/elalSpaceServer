using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElAlProjectCore.DTOs.RequstDTOs.AdminRequests
{
    public class AdminRequest_FlightDTO
    {
        public string FlightNumber { get; set; }
        public string DepartureAirport { get; set; }
        public string ArrivalAirport { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public int NumOfSeats { get; set; }
        public double Price { get; set; }
        public List<int> AmenityIds { get; set; } = new();
    }
}
