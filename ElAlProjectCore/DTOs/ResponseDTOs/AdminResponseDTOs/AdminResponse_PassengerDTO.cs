using ElAlProjectCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElAlProjectCore.DTOs.ResponseDTOs.AdminResponseDTOs
{
    public class AdminResponse_PassengerDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public List<AdminResponse_FlightDTO> Flights { get; set; }
    }
}
