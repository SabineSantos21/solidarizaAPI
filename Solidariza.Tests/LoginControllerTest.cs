using Microsoft.EntityFrameworkCore;
using Xunit;
using Moq;
using Solidariza.Controllers;
using Solidariza.Models;
using Solidariza.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Solidariza.Common;

namespace Solidariza.Tests
{
    public class LoginControllerTests
    {
        private readonly LoginController _controller;
        private readonly ConnectionDB _dbContext;
        private readonly IOptions<JwtSettings> _jwtSecret;

        public LoginControllerTests()
        {
            var options = new DbContextOptionsBuilder<ConnectionDB>()
               .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
               .Options;

            _dbContext = new ConnectionDB(options);

            _dbContext.Database.EnsureDeleted();
            _dbContext.Database.EnsureCreated();

            // Criando JwtSettings falso para testes
            _jwtSecret = Options.Create(new JwtSettings { SecretKey = "my_secret_key_which_is_long_enough" });

            SeedDatabase(_dbContext);

            _controller = new LoginController(_dbContext, _jwtSecret);
        }

        private static void SeedDatabase(ConnectionDB dbContext)
        {
            dbContext.User.Add(new User
            {
                UserId = 1,
                Name = "Test User",
                Email = "test@example.com",
                Password = "password" // Note que isso não está sendo usado no controller (hash é criado direto no método)
            });
            dbContext.SaveChanges();
        }

        [Fact]
        public void Login_ReturnsBadRequest_WhenPasswordIsNullOrEmpty()
        {
            var login = new Login { Email = "test@example.com", Password = "" };

            var result = _controller.Login(login);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("A senha é obrigatória.", badRequest.Value);
        }

        [Fact]
        public void Login_ReturnsBadRequest_WhenEmailIsNullOrEmpty()
        {
            var login = new Login { Email = "", Password = "senha123" };

            var result = _controller.Login(login);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("A email é obrigatória.", badRequest.Value);
        }

        [Fact]
        public void Login_ReturnsBadRequest_WhenUserNotFound()
        {
            var login = new Login { Email = "notfound@example.com", Password = "senha123" };

            var result = _controller.Login(login);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void Login_ReturnsOk_WhenUserIsValid()
        {
            var login = new Login { Email = "test@example.com", Password = "password" };

            var result = _controller.Login(login);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var loginResponse = Assert.IsType<LoginResponse>(okResult.Value);

            Assert.Equal("test@example.com", loginResponse.User.Email);
            Assert.False(string.IsNullOrEmpty(loginResponse.Token));
        }
    }
}
