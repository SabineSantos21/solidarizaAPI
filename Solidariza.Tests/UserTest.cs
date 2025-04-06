using Microsoft.EntityFrameworkCore;
using Xunit;
using Moq;
using Solidariza.Controllers;
using Solidariza.Models;
using Solidariza.Services;
using Microsoft.AspNetCore.Mvc;
using Solidariza.Models.Enum;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Solidariza.Tests
{
    public class UserControllerTests
    {
        private readonly UserController _controller;
        private readonly Mock<UserService> _mockUserService;
        private readonly ConnectionDB _dbContext;

        public UserControllerTests()
        {
            var options = new DbContextOptionsBuilder<ConnectionDB>()
                 .UseInMemoryDatabase(databaseName: "TestDatabase")
                 .Options;

            _dbContext = new ConnectionDB(options);

            // Seed Data se necessário
            SeedDatabase(_dbContext);

            _controller = new UserController(_dbContext);
        }

        private void SeedDatabase(ConnectionDB dbContext)
        {
            // Adicione dados de teste ao banco de dados em memória
            dbContext.User.Add(new User { UserId = 1, Name = "Test User" });
            dbContext.SaveChanges();
        }

        // Teste simplificado para CreateUser - Assumindo que seus serviços lançam exceções para facilitar o mock.

        [Fact]
        public async Task CreateUser_ValidUser_ReturnsOkResult()
        {
            // Arrange
            var newUser = new NewUser
            {
                Name = "New User",
                Email = "newuser@example.com",
                Type = (int)UserType.Standard,
                Password = "password"
            };

            // Act
            var result = await _controller.CreateUser(newUser);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<User>(actionResult.Value);
            Assert.Equal("New User", returnValue.Name);
        }
    }
}