using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElAlProjectCore.DTOs.RequstDTOs
{
    public class AuthRequestDTO
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(256)]
        public string Email { get; set; }
    }
}
