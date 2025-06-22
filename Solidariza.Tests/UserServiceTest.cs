using Microsoft.EntityFrameworkCore;
using Solidariza.Models;
using Solidariza.Models.Enum;
using Solidariza.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Solidariza.Tests.Services
{
    public class UserServiceTests
    {
        private readonly ConnectionDB _dbContext;
        private readonly UserService _service;

        public UserServiceTests()
        {
            var options = new DbContextOptionsBuilder<ConnectionDB>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new ConnectionDB(options);
            _dbContext.Database.EnsureDeleted();
            _dbContext.Database.EnsureCreated();

            _service = new UserService(_dbContext);
        }

        private void SeedDatabase()
        {
            var users = new[]
            {
                new User
                {
                    UserId = 1,
                    Name = "User1",
                    Email = "user1@example.com",
                    Password = "password1",
                    Type = UserType.Standard,
                    CreationDate = DateTime.UtcNow,
                    IsActive = true
                },
                new User
                {
                    UserId = 2,
                    Name = "User2",
                    Email = "user2@example.com",
                    Password = "password2",
                    Type = UserType.Organization,
                    DocumentNumber = "12345678901234",
                    DocumentType = DocumentType.CNPJ,
                    Phone = "1234567890",
                    CreationDate = DateTime.UtcNow,
                    IsActive = true
                }
            };
            _dbContext.User.AddRange(users);
            _dbContext.SaveChanges();
        }

        [Fact]
        public async Task GetUsers_EmptyDatabase_ReturnsEmptyList()
        {
            var result = await _service.GetUsers();

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetUsers_PopulatedDatabase_ReturnsAllUsers()
        {
            SeedDatabase();

            var result = await _service.GetUsers();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, u => u.UserId == 1 && u.Name == "User1");
            Assert.Contains(result, u => u.UserId == 2 && u.Name == "User2");
        }

        [Fact]
        public async Task GetUserById_ExistingId_ReturnsUser()
        {
            SeedDatabase();

            var result = await _service.GetUserById(1);

            Assert.NotNull(result);
            Assert.Equal(1, result.UserId);
            Assert.Equal("User1", result.Name);
            Assert.Equal("user1@example.com", result.Email);
        }

        [Fact]
        public async Task GetUserById_NonExistingId_ReturnsNull()
        {
            SeedDatabase();

            var result = await _service.GetUserById(999);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetUserByUserName_ExistingName_ReturnsUser()
        {
            SeedDatabase();

            var result = await _service.GetUserByUserName("User2");

            Assert.NotNull(result);
            Assert.Equal("User2", result.Name);
            Assert.Equal(2, result.UserId);
            Assert.Equal("user2@example.com", result.Email);
        }

        [Fact]
        public async Task GetUserByUserName_NonExistingName_ReturnsNull()
        {
            SeedDatabase();

            var result = await _service.GetUserByUserName("NonExistent");

            Assert.Null(result);
        }

        [Fact]
        public async Task GetUserByEmail_ExistingEmail_ReturnsUser()
        {
            SeedDatabase();

            var result = await _service.GetUserByEmail("user1@example.com");

            Assert.NotNull(result);
            Assert.Equal("user1@example.com", result.Email);
            Assert.Equal(1, result.UserId);
            Assert.Equal("User1", result.Name);
        }

        [Fact]
        public async Task GetUserByEmail_NonExistingEmail_ReturnsNull()
        {
            SeedDatabase();

            var result = await _service.GetUserByEmail("nonexistent@example.com");

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateUser_ValidNewUserWithDocumentType_CreatesUser()
        {
            var newUser = new NewUser
            {
                Name = "NewUser",
                Email = "newuser@example.com",
                Password = "password",
                Type = (int)UserType.Organization,
                DocumentNumber = "98765432109876",
                DocumentType = (int)DocumentType.CNPJ,
                Phone = "9876543210"
            };

            var result = await _service.CreateUser(newUser);

            Assert.NotNull(result);
            Assert.Equal("NewUser", result.Name);
            Assert.Equal("newuser@example.com", result.Email);
            Assert.Equal(UserType.Organization, result.Type);
            Assert.Equal("98765432109876", result.DocumentNumber);
            Assert.Equal(DocumentType.CNPJ, result.DocumentType);
            Assert.Equal("9876543210", result.Phone);
            Assert.Equal("password", result.Password);
            Assert.True(result.IsActive);
            Assert.True(DateTime.UtcNow.Subtract(result.CreationDate).TotalSeconds < 5);

            var dbUser = await _dbContext.User.FirstOrDefaultAsync(u => u.Email == "newuser@example.com");
            Assert.NotNull(dbUser);
            Assert.Equal(result.UserId, dbUser.UserId);
        }

        [Fact]
        public async Task CreateUser_ValidNewUserWithoutDocumentType_CreatesUser()
        {
            var newUser = new NewUser
            {
                Name = "NewUser",
                Email = "newuser@example.com",
                Password = "password",
                Type = (int)UserType.Standard,
                DocumentNumber = null,
                DocumentType = null,
                Phone = null
            };

            var result = await _service.CreateUser(newUser);

            Assert.NotNull(result);
            Assert.Equal("NewUser", result.Name);
            Assert.Equal("newuser@example.com", result.Email);
            Assert.Equal(UserType.Standard, result.Type);
            Assert.Null(result.DocumentNumber);
            Assert.Null(result.DocumentType);
            Assert.Null(result.Phone);
            Assert.Equal("password", result.Password);
            Assert.True(result.IsActive);
            Assert.True(DateTime.UtcNow.Subtract(result.CreationDate).TotalSeconds < 5);

            var dbUser = await _dbContext.User.FirstOrDefaultAsync(u => u.Email == "newuser@example.com");
            Assert.NotNull(dbUser);
            Assert.Equal(result.UserId, dbUser.UserId);
        }
    }
}