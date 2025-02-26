using Solidariza.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Solidariza.Services
{
    public class LoginService
    {
        private readonly ConnectionDB _dbContext;

        public LoginService(ConnectionDB dbContext) 
        {
            _dbContext = dbContext;
        }

        public string GerarTokenJWT(string email)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("hrjendj372849fnwyd7349299kjdu8nbcoabe99"));
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

        public User? ValidarCredenciais(string email, string senha)
        {
            return _dbContext.TbUsuario.FirstOrDefault(u => u.Email == email && u.Senha == senha);
        }
    }
}
