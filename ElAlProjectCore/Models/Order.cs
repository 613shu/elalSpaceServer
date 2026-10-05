using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElAlProjectCore.Enums;

namespace ElAlProjectCore.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int FlightId { get; set; }
        public Flight Flight { get; set; }
        public int PassengerId { get; set; }
        public Passenger Passenger { get; set; }
        public DateTime OrderDateTime { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime? CancelledAt { get; set; }
    }
}
