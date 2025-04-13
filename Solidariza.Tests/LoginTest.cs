using Microsoft.EntityFrameworkCore;
using Xunit;
using Moq;
using Solidariza.Controllers;
using Solidariza.Models;
using Solidariza.Services;
using Microsoft.AspNetCore.Mvc;

namespace Solidariza.Tests
{
    public class LoginControllerTests
    {
        private readonly LoginController _controller;
        private readonly Mock<LoginService> _mockLoginService;
        private readonly ConnectionDB _dbContext;

        public LoginControllerTests()
        {
            var options = new DbContextOptionsBuilder<ConnectionDB>()
               .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
               .Options;

            _dbContext = new ConnectionDB(options);

            _dbContext.Database.EnsureDeleted();
            _dbContext.Database.EnsureCreated();

            SeedDatabase(_dbContext);

            _mockLoginService = new Mock<LoginService>(_dbContext);
            _controller = new LoginController(_dbContext);
        }

        private void SeedDatabase(ConnectionDB dbContext)
        {
            dbContext.User.Add(new User
            {
                UserId = 1,
                Name = "Test User",
                Email = "test@example.com",
                Password = "password"
            });
            dbContext.SaveChanges();
        }

        [Fact]
        public void Login_ValidCredentials_ReturnsOkResult()
        {
            // Arrange
            var login = new Login { Email = "test@example.com", Password = "password" };

            // Mocking the behavior for successful login
            _mockLoginService.Setup(x => x.ValidarCredenciais(login.Email, login.Password))
                             .Returns(new User { UserId = 1, Name = "Test User" });
            _mockLoginService.Setup(x => x.GerarTokenJWT(login.Email))
                             .Returns("fake-jwt-token");

            var controller = new LoginController(_dbContext);

            // Act
            var result = controller.Login(login);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            var loginResponse = Assert.IsType<LoginResponse>(okResult.Value);
            Assert.Equal("Test User", loginResponse.User.Name);
            Assert.Equal("fake-jwt-token", loginResponse.Token);
        }

        [Fact]
        public void Login_InvalidCredentials_ReturnsBadRequest()
        {
            // Arrange
            var login = new Login { Email = "invalid@example.com", Password = "wrongpassword" };

            // Mocking the behavior for unsuccessful login
            _mockLoginService.Setup(x => x.ValidarCredenciais(login.Email, login.Password))
                             .Returns((User)null);

            var controller = new LoginController(_dbContext);

            // Act
            var result = controller.Login(login);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }
    }
}