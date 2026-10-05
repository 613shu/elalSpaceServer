using ElAlProjectCore.DTOs.ResponseDTOs;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ElAlProjectApi.Helpers
{
    public class AuthHelper
    {
        public static string CreateToken(LoginResultDTO loginres, IConfiguration configuration)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, loginres.Id.ToString()),
                new Claim(ClaimTypes.Name , loginres.Name),
                new Claim(ClaimTypes.Email, loginres.Email),
                new Claim(ClaimTypes.Role, loginres.Role)
            };

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("Jwt:Key")));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var tokeOptions = new JwtSecurityToken(
                issuer: configuration.GetValue<string>("Jwt:Issuer"),
                audience: configuration.GetValue<string>("Jwt:Audience"),
                claims: claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: signinCredentials
            );
            return new JwtSecurityTokenHandler().WriteToken(tokeOptions);
        }
    }
}
