using Solidariza.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Solidariza.Common;

namespace Solidariza.Services
{
    public class LoginService
    {
        private readonly ConnectionDB _dbContext;
        private readonly string _jwtSecret;

        public LoginService(ConnectionDB dbContext, IOptions<JwtSettings> jwtOptions) 
        {
            _dbContext = dbContext;
            _jwtSecret = jwtOptions.Value.SecretKey;
        }

        public string GerarTokenJWT(string email)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(ClaimTypes.Name, email)
            };

            var token = new JwtSecurityToken(
                "Solidariza",
                "Application",
                claims,
                expires: DateTime.UtcNow.AddHours(4),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public User? ValidarCredenciais(string email)
        {
            return _dbContext.User.FirstOrDefault(u => u.Email == email);
        }
    }
}
