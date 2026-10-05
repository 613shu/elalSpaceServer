using ElAlProjectCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElAlProjectCore.DTOs.ResponseDTOs.AdminResponseDTOs
{
    public class AdminResponse_OrderDTO
    {
        public int Id { get; set; }
        public AdminResponse_FlightDTO Flight { get; set; }
        public AdminResponse_PassengerDTO Passenger { get; set; }
        public DateTime OrderDateTime { get; set; }
        public string Status { get; set; }
    }
}
