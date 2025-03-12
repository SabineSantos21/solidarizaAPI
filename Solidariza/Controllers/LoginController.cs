using Solidariza;
using Solidariza.Services;
using Microsoft.AspNetCore.Mvc;
using Solidariza.Models;

namespace Solidariza.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly ConnectionDB _dbContext;

        public LoginController(ConnectionDB dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        public IActionResult Login(Login login)
        {
            LoginService loginService = new LoginService(_dbContext);

            User user = loginService.ValidarCredenciais(login.Email, login.Password);

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