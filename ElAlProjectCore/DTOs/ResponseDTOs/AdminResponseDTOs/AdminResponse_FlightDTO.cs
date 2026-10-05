using ElAlProjectCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElAlProjectCore.DTOs.ResponseDTOs.AdminResponseDTOs
{
    public class AdminResponse_FlightDTO
    {
        public int Id { get; set; }
        public string FlightNumber { get; set; }
        public string DepartureAirport { get; set; }
        public string ArrivalAirport { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public int NumOfSeats { get; set; }
        public int AvailableSeats { get; set; }
        public List<AdminResponse_PassengerDTO> Passengers { get; set; }
        public List<AdminResponse_AmenityDTO> Amenities { get; set; }
        public string FlightStatus { get; set; }
        public double Price { get; set; }


    }
}
