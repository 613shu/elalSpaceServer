using ElAlProjectApi.Helpers;
using ElAlProjectCore.DTOs.RequstDTOs;
using ElAlProjectCore.DTOs.ResponseDTOs;
using ElAlProjectCore.Models;
using ElAlProjectCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ElAlProjectApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {

        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;

        public AuthController(IAuthService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
        }



        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDTO<LoginResultDTO>>> Login(LoginModel loginModel, CancellationToken cancellationToken)
        {
            var profile = await _authService.LoginAsync(loginModel, cancellationToken);


            if (profile == null)
                return Unauthorized("Email or password is incorrect.");
            var token = AuthHelper.CreateToken(profile, _configuration);

            return Ok(new AuthResponseDTO<LoginResultDTO>
            {
                Token = token,
                Profile = profile
            });
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDTO<LoginResultDTO>>> Register([FromBody] AuthRequestDTO authRequest, CancellationToken cancellationToken)
        {
            var profile = await _authService.RegisterAsync(authRequest, cancellationToken);
            var token = AuthHelper.CreateToken(profile, _configuration);

            return Created($"api/passengers/{profile.Id}", new AuthResponseDTO<LoginResultDTO>
            {
                Token = token,
                Profile = profile
            });
        }

    }
}
