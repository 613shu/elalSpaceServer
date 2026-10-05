using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElAlProjectCore.DTOs.ResponseDTOs
{
    public class AuthResponseDTO<TProfile>
    {
        public string? Token { get; set; }
        public TProfile Profile { get; set; }
    }
}
