using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElAlProjectCore.Enums;

namespace ElAlProjectCore.Models
{
    public class Flight
    {
        public int Id { get; set; }
        public string FlightNumber { get; set; }
        public string DepartureAirport { get; set; }
        public string ArrivalAirport { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public int NumOfSeats { get; set; }
        public int AvailableSeats { get; set; }
        public List<Passenger> Passengers { get; set; }
        public List<Amenity> Amenities { get; set; }
        public FlightStatus FlightStatus { get; set; }
        public double Price { get; set; }


    }
}
