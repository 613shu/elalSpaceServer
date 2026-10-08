using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElAlProjectCore.DTOs.RequstDTOs.AdminRequests
{
    public class AdminRequest_AmenityDTO
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
    }
}
