using Microsoft.EntityFrameworkCore;
using Xunit;
using Solidariza.Controllers;
using Solidariza.Models;
using Solidariza.Models.Enum;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using Solidariza.Services;

namespace Solidariza.Tests
{
    public class ProfileControllerTests
    {
        private readonly ProfileController _controller;
        private readonly ConnectionDB _dbContext;

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
            dbContext.Profile.Add(new Profile
            {
                ProfileId = 1,
                Name = "Profile 1",
                Phone = "123456789",
                Description = "Description 1",
                Address = "Address 1",
                City = "City 1",
                State = "State 1",
                Zip = "00000-000",
                UserId = 1,
            });

            dbContext.Profile.Add(new Profile
            {
                ProfileId = 2,
                Name = "Profile 2",
                Phone = "987654321",
                Description = "Description 2",
                Address = "Address 2",
                City = "City 2",
                State = "State 2",
                Zip = "11111-111",
                UserId = 2,
            });

            dbContext.SaveChanges();
        }

        [Fact]
        public async Task GetProfileById_ReturnsProfile_WhenExists()
        {
            var result = await _controller.GetProfileById(1);

            var okResult = Assert.IsType<ActionResult<Profile>>(result);
            Assert.NotNull(okResult.Value);
            Assert.Equal(1, okResult.Value.ProfileId);
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
            Assert.NotNull(actionResult.Value);
            Assert.Equal(1, actionResult.Value.UserId);
        }

        [Fact]
        public async Task GetProfileByUserId_ReturnsNotFound_WhenDoesNotExist()
        {
            var result = await _controller.GetProfileByUserId(999);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreateProfile_ReturnsOk_WithCreatedProfile()
        {
            var newProfile = new NewProfile
            {
                Name = "New Profile",
                Phone = "000000000",
                Description = "New Description",
                Address = "New Address",
                City = "New City",
                State = "New State",
                Zip = "22222-222",
                UserId = 3,
            };

            var result = await _controller.CreateProfile(newProfile);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var createdProfile = Assert.IsType<Profile>(okResult.Value);
            Assert.Equal("New Profile", createdProfile.Name);
        }

        [Fact]
        public async Task PutProfile_ReturnsNoContent_WhenProfileExists()
        {
            var updateProfile = new UpdateProfile
            {
                Name = "Updated Name",
                Phone = "555555555",
                Description = "Updated Description",
                Address = "Updated Address",
                City = "Updated City",
                State = "Updated State",
                Zip = "33333-333"
            };

            var result = await _controller.PutProfile(1, updateProfile);

            Assert.IsType<NoContentResult>(result);

            var updated = await _dbContext.Profile.FindAsync(1);
            Assert.Equal("Updated Name", updated.Name);
            Assert.Equal("555555555", updated.Phone);
        }

        [Fact]
        public async Task PutProfile_ReturnsNotFound_WhenProfileDoesNotExist()
        {
            var updateProfile = new UpdateProfile
            {
                Name = "Does not matter"
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
    }
}
