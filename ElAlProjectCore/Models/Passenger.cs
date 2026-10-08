using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElAlProjectCore.Models
{
    public class Passenger
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Passward { get; set; }
        public List<Order>? Orders { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
