using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElAlProjectCore.DTOs.ResponseDTOs.PassengerResonseDTOs
{
    public class PassengerResponse_FlightDTO
    {
        public int Id { get; set; }
        public string FlightNumber { get; set; }
        public string DepartureAirport { get; set; }
        public string ArrivalAirport { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public string FlightStatus { get; set; }

        public int NumOfSeats { get; set; }
        public int AvailableSeats { get; set; }
        public double Price { get; set; }
        public List<PassengerResponse_AmenityDTO> Amenities { get; set; }

    }
}
