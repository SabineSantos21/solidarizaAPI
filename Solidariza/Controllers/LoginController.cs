using Solidariza;
using Solidariza.Services;
using Microsoft.AspNetCore.Mvc;
using Solidariza.Models;
using Solidariza.Common;
using Microsoft.Extensions.Options;

namespace Solidariza.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly ConnectionDB _dbContext;
        private readonly IOptions<JwtSettings> _jwtSecret;

        public LoginController(ConnectionDB dbContext, IOptions<JwtSettings> jwtOptions)
        {
            _dbContext = dbContext;
            _jwtSecret = jwtOptions;
        }

        [HttpPost]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Login(Login login)
        {

            LoginService loginService = new LoginService(_dbContext,_jwtSecret);

            if (string.IsNullOrEmpty(login.Password))
            {
                return BadRequest("A senha é obrigatória.");
            }

            var hashPassword = PasswordHash.HashPassword(login.Password);
            var verifyPassword = PasswordHash.VerifyPassword(login.Password, hashPassword);
            
            if (!verifyPassword) return BadRequest();

            if (string.IsNullOrEmpty(login.Email))
            {
                return BadRequest("A email é obrigatória.");
            }

            User? user = loginService.ValidarCredenciais(login.Email);

            if ( user != null)
            {
                UserResponse userResponse = new UserResponse()
                {
                    UserId = user.UserId,
                    Name = user.Name,
                    DocumentNumber = user.DocumentNumber,
                    DocumentType = user.DocumentType,
                    Type = user.Type,
                    Email = user.Email,
                    IsActive = user.IsActive,
                    Phone = user.Phone,
                };

                var token = loginService.GerarTokenJWT(login.Email);
                return Ok(new LoginResponse() { 
                    User = userResponse,
                    Token = token 
                });
            }

            return BadRequest();
        }
    }
}