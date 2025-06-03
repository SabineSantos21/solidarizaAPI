using Microsoft.EntityFrameworkCore;
using Xunit;
using Moq;
using Solidariza.Controllers;
using Solidariza.Models;
using Solidariza.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using Solidariza.Models.Enum;
using System.Linq;

namespace Solidariza.Tests
{
    public class ProfileControllerTests
    {
        private readonly ConnectionDB _dbContext;
        private readonly ProfileController _controller;

        public ProfileControllerTests()
        {
            var options = new DbContextOptionsBuilder<ConnectionDB>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new ConnectionDB(options);

            _dbContext.Database.EnsureDeleted();
            _dbContext.Database.EnsureCreated();

            SeedDatabase(_dbContext);

            _controller = new ProfileController(_dbContext);
        }

        private static void SeedDatabase(ConnectionDB dbContext)
        {
            var userOrg = new User
            {
                UserId = 1,
                Name = "Org User",
                Email = "org@example.com",
                Password = "pass",
                Type = UserType.Organization
            };

            var userVol = new User
            {
                UserId = 2,
                Name = "Volunteer User",
                Email = "vol@example.com",
                Password = "pass",
                Type = UserType.Volunteer
            };

            dbContext.User.AddRange(userOrg, userVol);

            dbContext.Profile.Add(new Profile
            {
                ProfileId = 1,
                Name = "Org Profile",
                Phone = "1234567890",
                Description = "Org Desc",
                Address = "123 Org Street",
                City = "Org City",
                State = "OS",
                Zip = "12345-678",
                UserId = userOrg.UserId,
                User = userOrg
            });

            dbContext.Profile.Add(new Profile
            {
                ProfileId = 2,
                Name = "Vol Profile",
                Phone = "9876543210",
                Description = "Vol Desc",
                Address = "456 Vol Ave",
                City = "Vol City",
                State = "VS",
                Zip = "98765-432",
                UserId = userVol.UserId,
                User = userVol
            });

            dbContext.SaveChanges();
        }

        [Fact]
        public async Task GetProfileById_ReturnsProfile_WhenExists()
        {
            var result = await _controller.GetProfileById(1);

            var okResult = Assert.IsType<ActionResult<Profile>>(result);
            var profile = Assert.IsType<Profile>(okResult.Value);
            Assert.NotNull(profile);
            Assert.Equal(1, profile.ProfileId);
            Assert.Equal("Org Profile", profile.Name);
            Assert.Equal("1234567890", profile.Phone);
        }

        [Fact]
        public async Task GetProfileById_ReturnsNotFound_WhenDoesNotExist()
        {
            var result = await _controller.GetProfileById(999);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetProfileByUserId_ReturnsProfile_WhenExists()
        {
            var result = await _controller.GetProfileByUserId(1);

            var actionResult = Assert.IsType<ActionResult<Profile>>(result);
            var profile = Assert.IsType<Profile>(actionResult.Value);
            Assert.NotNull(profile);
            Assert.Equal(1, profile.UserId);
            Assert.Equal("Org Profile", profile.Name);
            Assert.Equal("1234567890", profile.Phone);
        }

        [Fact]
        public async Task GetProfileByUserId_ReturnsNotFound_WhenDoesNotExist()
        {
            var result = await _controller.GetProfileByUserId(999);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetProfileByUserId_ThrowsException_ReturnsProblem()
        {
            var controller = new ProfileController(null!); // Força null para causar exceção

            var result = await controller.GetProfileByUserId(1);

            var problemResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, problemResult.StatusCode);
            Assert.IsType<ProblemDetails>(problemResult.Value);
        }

        [Fact]
        public async Task CreateProfile_ReturnsOk_WithCreatedProfile()
        {
            var newUser = new User
            {
                UserId = 3,
                Name = "New User",
                Email = "new@example.com",
                Password = "pass",
                Type = UserType.Volunteer
            };
            _dbContext.User.Add(newUser);
            _dbContext.SaveChanges();

            var newProfile = new NewProfile
            {
                Name = "New Profile",
                Phone = "5555555555",
                Description = "New Description",
                Address = "789 New St",
                City = "New City",
                State = "NS",
                Zip = "55555-555",
                UserId = 3
            };

            var result = await _controller.CreateProfile(newProfile);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var createdProfile = Assert.IsType<Profile>(okResult.Value);
            Assert.Equal("New Profile", createdProfile.Name);
            Assert.Equal("5555555555", createdProfile.Phone);
            Assert.Equal(3, createdProfile.UserId);

            var dbProfile = await _dbContext.Profile.FirstOrDefaultAsync(p => p.UserId == 3);
            Assert.NotNull(dbProfile);
            Assert.Equal("New Profile", dbProfile.Name);
        }

        [Fact]
        public async Task CreateProfile_ThrowsException_ReturnsProblem()
        {
            var controller = new ProfileController(null!); // Força null para causar exceção
            var newProfile = new NewProfile();

            var result = await controller.CreateProfile(newProfile);

            var problemResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, problemResult.StatusCode);
            Assert.IsType<ProblemDetails>(problemResult.Value);
        }

        [Fact]
        public async Task PutProfile_ReturnsNoContent_WhenProfileExists()
        {
            var updateProfile = new UpdateProfile
            {
                Name = "Updated Name",
                Phone = "5555555555",
                Description = "Updated Description",
                Address = "Updated Address",
                City = "Updated City",
                State = "US",
                Zip = "33333-333"
            };

            var result = await _controller.PutProfile(1, updateProfile);

            Assert.IsType<NoContentResult>(result);

            var updated = await _dbContext.Profile.FindAsync(1);
            Assert.NotNull(updated);
            Assert.Equal("Updated Name", updated.Name);
            Assert.Equal("5555555555", updated.Phone);
            Assert.Equal("Updated Address", updated.Address);
            Assert.Equal("Updated City", updated.City);
            Assert.Equal("US", updated.State);
            Assert.Equal("33333-333", updated.Zip);
        }

        [Fact]
        public async Task PutProfile_ReturnsNotFound_WhenProfileDoesNotExist()
        {
            var updateProfile = new UpdateProfile
            {
                Name = "Updated Name",
                Phone = "5555555555",
                Description = "Updated Description",
                Address = "Updated Address",
                City = "Updated City",
                State = "US",
                Zip = "33333-333"
            };

            var result = await _controller.PutProfile(999, updateProfile);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteProfile_ReturnsNoContent_WhenProfileExists()
        {
            var result = await _controller.DeleteProfile(2);

            Assert.IsType<NoContentResult>(result);

            var deleted = await _dbContext.Profile.FindAsync(2);
            Assert.Null(deleted);
        }

        [Fact]
        public async Task DeleteProfile_ReturnsNotFound_WhenProfileDoesNotExist()
        {
            var result = await _controller.DeleteProfile(999);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetProfilesOrganization_ReturnsOk_WithOrganizationProfiles()
        {
            var result = await _controller.GetProfilesOrganization();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var profiles = Assert.IsType<List<Profile>>(okResult.Value);
            Assert.NotEmpty(profiles);
            Assert.All(profiles, p => Assert.Equal(UserType.Organization, p.User.Type));
            Assert.Single(profiles); // Deve retornar apenas o perfil com UserType.Organization
            Assert.Equal("Org Profile", profiles[0].Name);
        }

        [Fact]
        public async Task GetProfilesOrganization_ThrowsException_ReturnsProblem()
        {
            var controller = new ProfileController(null!); // Força null para causar exceção

            var result = await controller.GetProfilesOrganization();

            var problemResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, problemResult.StatusCode);
            Assert.IsType<ProblemDetails>(problemResult.Value);
        }
    }
}