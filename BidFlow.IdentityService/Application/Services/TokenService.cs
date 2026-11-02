using BidFlow.IdentityService.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BidFlow.IdentityService.Application.Services
{
    public class TokenService
    {
        private readonly IConfiguration _configuration;
        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string GenerateToken(AppUser user)
        {
            var claims  = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email,user.Email),
                new Claim (ClaimTypes.Name,user.UserName)
            };
            var key = _configuration["Jwt:Key"]
                ?? throw new InvalidOperationException(
                    "JWT signing key is not configured.");
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(symmetricSecurityKey,SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(issuer:_configuration["Jwt:Issuer"], 
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Jwt:AccessTokenExpirationMinutes"])),
                signingCredentials : credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
