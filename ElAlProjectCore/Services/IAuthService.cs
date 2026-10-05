using ElAlProjectCore.DTOs.RequstDTOs;
using ElAlProjectCore.DTOs.ResponseDTOs;
using ElAlProjectCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElAlProjectCore.Services
{
    public interface IAuthService
    {

        Task<LoginResultDTO> LoginAsync(LoginModel loginModel, CancellationToken cancellationToken);

        Task<LoginResultDTO> RegisterAsync(AuthRequestDTO authRequest, CancellationToken cancellationToken);



    }
}
