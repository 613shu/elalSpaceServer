using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElAlProjectCore.DTOs.RequstDTOs.AdminRequests
{
    public class AdminRequest_FlightDTO
    {
        [Required]
        [StringLength(20)]
        public string FlightNumber { get; set; }

        [Required]
        [StringLength(100)]
        public string DepartureAirport { get; set; }

        [Required]
        [StringLength(100)]
        public string ArrivalAirport { get; set; }

        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }

        [Range(1, 1000)]
        public int NumOfSeats { get; set; }

        [Range(0.01, double.MaxValue)]
        public double Price { get; set; }

        public List<int> AmenityIds { get; set; } = new();
    }
}
