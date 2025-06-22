using Microsoft.EntityFrameworkCore;
using Xunit;
using Moq;
using Solidariza.Controllers;
using Solidariza.Models;
using Solidariza.Services;
using Microsoft.AspNetCore.Mvc;
using Solidariza.Models.Enum;
using Solidariza.Common;
using System.Threading.Tasks;
using System;
using Solidariza.Interfaces.Services;

namespace Solidariza.Tests
{
    public class UserControllerTests
    {
        private readonly ConnectionDB _dbContext;
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<IProfileService> _mockProfileService;
        private readonly Mock<IValidateOrganizationService> _mockValidateOrganizationService;
        private readonly Mock<IOrganizationInfoService> _mockOrganizationInfoService;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            var options = new DbContextOptionsBuilder<ConnectionDB>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new ConnectionDB(options);

            _dbContext.Database.EnsureDeleted();
            _dbContext.Database.EnsureCreated();

            SeedDatabase(_dbContext);

            _mockUserService = new Mock<IUserService>();
            _mockProfileService = new Mock<IProfileService>();
            _mockValidateOrganizationService = new Mock<IValidateOrganizationService>();
            _mockOrganizationInfoService = new Mock<IOrganizationInfoService>();

            _controller = new UserController(
                _mockUserService.Object,
                _mockProfileService.Object,
                _mockValidateOrganizationService.Object,
                _mockOrganizationInfoService.Object);
        }

        private static void SeedDatabase(ConnectionDB dbContext)
        {
            dbContext.User.Add(new User
            {
                UserId = 1,
                Name = "Test User",
                Email = "test@example.com",
                Password = PasswordHash.HashPassword("password"),
                Type = UserType.Standard
            });
            dbContext.SaveChanges();
        }

        [Fact]
        public async Task GetUserById_ExistingId_ReturnsUser()
        {
            var user = new User { UserId = 1, Name = "Test User", Email = "test@example.com" };
            _mockUserService.Setup(s => s.GetUserById(1)).ReturnsAsync(user);

            var result = await _controller.GetUserById(1);

            var okResult = Assert.IsType<ActionResult<User>>(result);
            var returnedUser = Assert.IsType<User>(okResult.Value);
            Assert.Equal(1, returnedUser.UserId);
            Assert.Equal("Test User", returnedUser.Name);
        }

        [Fact]
        public async Task GetUserById_NonExistingId_ReturnsNotFound()
        {
            _mockUserService.Setup(s => s.GetUserById(999)).ReturnsAsync((User?)null);

            var result = await _controller.GetUserById(999);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task PostUser_ValidStandardUser_ReturnsOk()
        {
            var newUser = new NewUser
            {
                Name = "New User",
                Email = "newuser@example.com",
                Password = "password",
                Type = (int)UserType.Standard
            };
            var createdUser = new User
            {
                UserId = 2,
                Name = "New User",
                Email = "newuser@example.com",
                Password = PasswordHash.HashPassword("password"),
                Type = UserType.Standard
            };
            var createdProfile = new Profile { ProfileId = 1, UserId = 2, Name = "New User" };

            _mockUserService.Setup(s => s.GetUserByEmail("newuser@example.com")).ReturnsAsync((User?)null);
            _mockUserService.Setup(s => s.CreateUser(It.IsAny<NewUser>())).ReturnsAsync(createdUser);
            _mockProfileService.Setup(s => s.CreateProfile(It.IsAny<NewProfile>())).ReturnsAsync(createdProfile);

            var result = await _controller.PostUser(newUser);

            var actionResult = Assert.IsType<ActionResult<User>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedUser = Assert.IsType<User>(okResult.Value);
            Assert.Equal("New User", returnedUser.Name);
            Assert.Equal("newuser@example.com", returnedUser.Email);
            _mockProfileService.Verify(s => s.CreateProfile(It.Is<NewProfile>(p => p.UserId == 2 && p.Name == "New User")), Times.Once());
        }

        [Fact]
        public async Task PostUser_ValidOrganizationUser_ValidCNPJ_ReturnsOk()
        {
            var newUser = new NewUser
            {
                Name = "Org User",
                Email = "org@example.com",
                Password = "password",
                Type = (int)UserType.Organization,
                DocumentNumber = "12345678901234"
            };
            var createdUser = new User
            {
                UserId = 3,
                Name = "Org User",
                Email = "org@example.com",
                Password = PasswordHash.HashPassword("password"),
                Type = UserType.Organization,
                DocumentNumber = "12345678901234"
            };
            var createdProfile = new Profile { ProfileId = 2, UserId = 3, Name = "Org User" };
            var cnpjResponse = new ConsultCnpjResponse { IsValid = true, DisapprovalReason = null };

            _mockUserService.Setup(s => s.GetUserByEmail("org@example.com")).ReturnsAsync((User?)null);
            _mockUserService.Setup(s => s.CreateUser(It.IsAny<NewUser>())).ReturnsAsync(createdUser);
            _mockProfileService.Setup(s => s.CreateProfile(It.IsAny<NewProfile>())).ReturnsAsync(createdProfile);
            _mockValidateOrganizationService.Setup(s => s.ConsultCNPJ("12345678901234")).ReturnsAsync(cnpjResponse);

            var result = await _controller.PostUser(newUser);

            var actionResult = Assert.IsType<ActionResult<User>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedUser = Assert.IsType<User>(okResult.Value);
            Assert.Equal("Org User", returnedUser.Name);
            Assert.Equal("org@example.com", returnedUser.Email);
            _mockProfileService.Verify(s => s.CreateProfile(It.Is<NewProfile>(p => p.UserId == 3 && p.Name == "Org User")), Times.Once());
            _mockValidateOrganizationService.Verify(s => s.ConsultCNPJ("12345678901234"), Times.Once());
            _mockOrganizationInfoService.Verify(s => s.CreateOrganizationInfoCPNJValid(It.Is<NewOrganizationInfoCnpjValid>(o => o.UserId == 3 && o.IsOrganizationApproved)), Times.Once());
        }

        [Fact]
        public async Task PostUser_ValidOrganizationUser_InvalidCNPJ_ReturnsOk()
        {
            var newUser = new NewUser
            {
                Name = "Org User",
                Email = "org@example.com",
                Password = "password",
                Type = (int)UserType.Organization,
                DocumentNumber = "00000000000000"
            };
            var createdUser = new User
            {
                UserId = 3,
                Name = "Org User",
                Email = "org@example.com",
                Password = PasswordHash.HashPassword("password"),
                Type = UserType.Organization,
                DocumentNumber = "00000000000000"
            };
            var createdProfile = new Profile { ProfileId = 2, UserId = 3, Name = "Org User" };
            var cnpjResponse = new ConsultCnpjResponse { IsValid = false, DisapprovalReason = "CNPJ inválido" };

            _mockUserService.Setup(s => s.GetUserByEmail("org@example.com")).ReturnsAsync((User?)null);
            _mockUserService.Setup(s => s.CreateUser(It.IsAny<NewUser>())).ReturnsAsync(createdUser);
            _mockProfileService.Setup(s => s.CreateProfile(It.IsAny<NewProfile>())).ReturnsAsync(createdProfile);
            _mockValidateOrganizationService.Setup(s => s.ConsultCNPJ("00000000000000")).ReturnsAsync(cnpjResponse);

            var result = await _controller.PostUser(newUser);

            var actionResult = Assert.IsType<ActionResult<User>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedUser = Assert.IsType<User>(okResult.Value);
            Assert.Equal("Org User", returnedUser.Name);
            Assert.Equal("org@example.com", returnedUser.Email);
            _mockProfileService.Verify(s => s.CreateProfile(It.Is<NewProfile>(p => p.UserId == 3 && p.Name == "Org User")), Times.Once());
            _mockValidateOrganizationService.Verify(s => s.ConsultCNPJ("00000000000000"), Times.Once());
            _mockOrganizationInfoService.Verify(s => s.CreateOrganizationInfoCPNJValid(It.Is<NewOrganizationInfoCnpjValid>(o => o.UserId == 3 && !o.IsOrganizationApproved && o.DisapprovalReason == "CNPJ inválido")), Times.Once());
        }

        [Fact]
        public async Task PostUser_EmptyEmail_ReturnsBadRequest()
        {
            var newUser = new NewUser
            {
                Name = "New User",
                Email = "",
                Password = "password",
                Type = (int)UserType.Standard
            };

            var result = await _controller.PostUser(newUser);

            var actionResult = Assert.IsType<ActionResult<User>>(result);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            Assert.Equal("O e-mail é obrigatório.", badRequestResult.Value);
        }

        [Fact]
        public async Task PostUser_DuplicateEmail_ReturnsBadRequest()
        {
            var newUser = new NewUser
            {
                Name = "New User",
                Email = "test@example.com",
                Password = "password",
                Type = (int)UserType.Standard
            };
            var existingUser = new User { UserId = 1, Email = "test@example.com" };

            _mockUserService.Setup(s => s.GetUserByEmail("test@example.com")).ReturnsAsync(existingUser);

            var result = await _controller.PostUser(newUser);

            var actionResult = Assert.IsType<ActionResult<User>>(result);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            Assert.Equal("Este e-mail não está mais disponível para cadastro. Por favor, utilize outro e-mail ou, se já possui um cadastro, recupere o acesso ao anterior.", badRequestResult.Value);
        }

        [Fact]
        public async Task PostUser_EmptyPassword_ReturnsBadRequest()
        {
            var newUser = new NewUser
            {
                Name = "New User",
                Email = "newuser@example.com",
                Password = "",
                Type = (int)UserType.Standard
            };

            _mockUserService.Setup(s => s.GetUserByEmail("newuser@example.com")).ReturnsAsync((User?)null);

            var result = await _controller.PostUser(newUser);

            var actionResult = Assert.IsType<ActionResult<User>>(result);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            Assert.Equal("A senha é obrigatória.", badRequestResult.Value);
        }

        [Fact]
        public async Task PostUser_OrganizationWithoutDocumentNumber_ReturnsBadRequest()
        {
            var newUser = new NewUser
            {
                Name = "Org User",
                Email = "org@example.com",
                Password = "password",
                Type = (int)UserType.Organization,
                DocumentNumber = null
            };
            var createdUser = new User
            {
                UserId = 3,
                Name = "Org User",
                Email = "org@example.com",
                Password = PasswordHash.HashPassword("password"),
                Type = UserType.Organization
            };
            var createdProfile = new Profile { ProfileId = 2, UserId = 3, Name = "Org User" };

            _mockUserService.Setup(s => s.GetUserByEmail("org@example.com")).ReturnsAsync((User?)null);
            _mockUserService.Setup(s => s.CreateUser(It.IsAny<NewUser>())).ReturnsAsync(createdUser);
            _mockProfileService.Setup(s => s.CreateProfile(It.IsAny<NewProfile>())).ReturnsAsync(createdProfile);

            var result = await _controller.PostUser(newUser);

            var actionResult = Assert.IsType<ActionResult<User>>(result);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            Assert.Equal("Número do documento é obrigatório para organizações.", badRequestResult.Value);
        }

        [Fact]
        public async Task PostUser_ThrowsException_ReturnsProblem()
        {
            var newUser = new NewUser
            {
                Name = "New User",
                Email = "newuser@example.com",
                Password = "password",
                Type = (int)UserType.Standard
            };

            _mockUserService.Setup(s => s.GetUserByEmail("newuser@example.com")).ThrowsAsync(new Exception("Database error"));

            var result = await _controller.PostUser(newUser);

            var actionResult = Assert.IsType<ActionResult<User>>(result);
            var problemResult = Assert.IsType<ObjectResult>(actionResult.Result);
            Assert.Equal(500, problemResult.StatusCode);
            Assert.IsType<ProblemDetails>(problemResult.Value);
            Assert.Equal("Database error", ((ProblemDetails)problemResult.Value).Detail);
        }
    }
}