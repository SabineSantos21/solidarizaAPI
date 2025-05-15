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
        private readonly Mock<LoginService> _mockLoginService;
        private readonly ConnectionDB _dbContext;
        private readonly IOptions<JwtSettings> _jwtSecret;

        public LoginControllerTests(IOptions<JwtSettings> jwtOptions)
        {
            var options = new DbContextOptionsBuilder<ConnectionDB>()
               .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
               .Options;

            _dbContext = new ConnectionDB(options);

            _dbContext.Database.EnsureDeleted();
            _dbContext.Database.EnsureCreated();

            _jwtSecret = jwtOptions;

            SeedDatabase(_dbContext);

            _mockLoginService = new Mock<LoginService>(_dbContext);
            _controller = new LoginController(_dbContext, _jwtSecret);
        }

        private static void SeedDatabase(ConnectionDB dbContext)
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

        

        
    }
}