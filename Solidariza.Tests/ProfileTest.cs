using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Solidariza.Controllers;
using Solidariza.Models;
using Solidariza.Services;

namespace Solidariza.Tests
{
    public class ProfileControllerTests
    {
        private readonly ProfileController _controller;
        private readonly ConnectionDB _dbContext;
        private readonly Mock<ProfileService> _mockProfileService;

        public ProfileControllerTests()
        {
            var options = new DbContextOptionsBuilder<ConnectionDB>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new ConnectionDB(options);
            _mockProfileService = new Mock<ProfileService>(_dbContext);

            _dbContext.Database.EnsureDeleted();
            _dbContext.Database.EnsureCreated();

            SeedDatabase(_dbContext);

            _controller = new ProfileController(_dbContext);
        }

        private void SeedDatabase(ConnectionDB dbContext)
        {
            if (!dbContext.Profile.Any())
            {
                dbContext.Profile.Add(new Profile
                {
                    Name = "Test Profile",
                    UserId = 1,
                    Description = "Description",
                    Address = "Rua teste",
                    City = "Joinville",
                    Phone = "(47)911111111",
                    State = "SC",
                    Zip = "89102933"
                });

                dbContext.SaveChanges();
            }
        }

        [Fact]
        public async Task GetProfileById_NonExistingId_ReturnsNotFound()
        {
            int testProfileId = 999999;
            var result = await _controller.GetProfileById(testProfileId);
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetProfileByUserId_ExistingUserId_ReturnsProfile()
        {
            int testUserId = 1;
            var result = await _controller.GetProfileByUserId(testUserId);
            Assert.IsType<ActionResult<Profile>>(result);
            Assert.NotNull(result.Value);
        }

        [Fact]
        public async Task GetProfileByUserId_NonExistingUserId_ReturnsNotFound()
        {
            int testUserId = 999999;
            var result = await _controller.GetProfileByUserId(testUserId);
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreateProfile_ValidNewProfile_ReturnsOk()
        {
            var newProfile = new NewProfile
            {
                Name = "New Profile",
                UserId = 2,
                Description = "New description",
                Address = "Rua Nova",
                City = "Florianópolis",
                Phone = "(47)922222222",
                State = "SC",
                Zip = "89103456"
            };

            var result = await _controller.CreateProfile(newProfile);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task PutProfile_ExistingId_UpdatesProfile()
        {
            int testProfileId = 1;
            var updateProfile = new UpdateProfile
            {
                Name = "Updated Profile",
                Phone = "(47)933333333",
                Description = "Updated Description",
                Address = "Rua Alterada",
                City = "São Paulo",
                State = "SP",
                Zip = "89107891"
            };

            var result = await _controller.PutProfile(testProfileId, updateProfile);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task PutProfile_NonExistingId_ReturnsNotFound()
        {
            int testProfileId = 999999;
            var updateProfile = new UpdateProfile
            {
                Name = "Updated Profile",
                Phone = "(47)933333333",
                Description = "Updated Description",
                Address = "Rua Alterada",
                City = "São Paulo",
                State = "SP",
                Zip = "89107891"
            };

            var result = await _controller.PutProfile(testProfileId, updateProfile);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteProfile_ExistingId_DeletesProfile()
        {
            int testProfileId = 1;
            var result = await _controller.DeleteProfile(testProfileId);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteProfile_NonExistingId_ReturnsNotFound()
        {
            int testProfileId = 999999;
            var result = await _controller.DeleteProfile(testProfileId);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetProfilesOrganization_ReturnsProfiles()
        {
            var result = await _controller.GetProfilesOrganization();
            Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(((OkObjectResult)result.Result).Value);
        }
    }
}