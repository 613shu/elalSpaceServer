using ElAlProjectCore.DTOs.ResponseDTOs.AdminResponseDTOs;
using ElAlProjectCore.DTOs.ResponseDTOs.PassengerResonseDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElAlProjectCore.DTOs.ResponseDTOs
{
    public class LoginResultDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }

       
        public AdminResponse_AdminDTO? AdminProfile { get; set; }
        public PassengerResponse_PassengerDTO? PassengerProfile { get; set; }

    }
}
