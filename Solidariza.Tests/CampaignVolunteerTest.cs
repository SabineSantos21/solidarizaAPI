using Microsoft.EntityFrameworkCore;
using Xunit;
using Moq;
using Solidariza.Controllers;
using Solidariza.Models;
using Solidariza.Services;
using Microsoft.AspNetCore.Mvc;
using Solidariza.Models.Enum;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Solidariza.Tests
{
    public class CampaignVolunteerControllerTests
    {
        private readonly CampaignVolunteerController _controller;
        private readonly ConnectionDB _dbContext;

        public CampaignVolunteerControllerTests()
        {
            var options = new DbContextOptionsBuilder<ConnectionDB>()
               .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
               .Options;

            _dbContext = new ConnectionDB(options);

            _dbContext.Database.EnsureDeleted();
            _dbContext.Database.EnsureCreated();

            SeedDatabase(_dbContext);

            _controller = new CampaignVolunteerController(_dbContext);
        }

        private void SeedDatabase(ConnectionDB dbContext)
        {
            if (!dbContext.Campaign_Volunteers.Any())
            {
                dbContext.Campaign_Volunteers.Add(new CampaignVolunteer
                {
                    CampaignId = 1,
                    UserId = 1,
                    IsApproved = CampaignVolunteerStatus.PENDING
                });

                dbContext.SaveChanges();
            }
        }

        [Fact]
        public async Task GetCampaignVolunteerById_NonExistingId_ReturnsNotFound()
        {
            int testId = 99;
            var result = await _controller.GetCampaignVolunteerById(testId);
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetCampaignVolunteerByCampaignId_ExistingCampaignId_ReturnsVolunteers()
        {
            int testCampaignId = 1;
            var result = await _controller.GetCampaignVolunteerByCampaignId(testCampaignId);
            var okResult = Assert.IsType<ActionResult<List<CampaignVolunteer>>>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task GetCampaignVolunteerByUserId_ExistingUserId_ReturnsVolunteers()
        {
            int testUserId = 1;
            var result = await _controller.GetCampaignVolunteerByUserId(testUserId);
            var okResult = Assert.IsType<ActionResult<List<CampaignVolunteer>>>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task GetCampaignVolunteerByUserIdAndAproved_ExistingUserId_ReturnsApprovedVolunteers()
        {
            int testUserId = 1;
            var result = await _controller.GetCampaignVolunteerByUserIdAndAproved(testUserId);
            var okResult = Assert.IsType<ActionResult<List<CampaignVolunteer>>>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task CreateCampaignVolunteer_NewVolunteer_ReturnsCreatedVolunteer()
        {
            var newVolunteer = new NewCampaignVolunteer
            {
                CampaignId = 2,
                UserId = 2
            };

            var result = await _controller.CreateCampaignVolunteer(newVolunteer);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var createdVolunteer = Assert.IsType<CampaignVolunteer>(okResult.Value);
            Assert.Equal(2, createdVolunteer.UserId);
        }

        [Fact]
        public async Task PutCampaignVolunteer_ExistingId_UpdatesVolunteerStatus()
        {
            int testId = 1;
            var updateVolunteer = new UpdateCampaignVolunteer
            {
                IsApproved = (int)CampaignVolunteerStatus.PENDING
            };

            var result = await _controller.PutCampaignVolunteer(testId, updateVolunteer);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteCampaignVolunteer_ExistingId_DeletesVolunteer()
        {
            int testId = 1;
            var result = await _controller.DeleteCampaignVolunteer(testId);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteCampaignVolunteer_NonExistingId_ReturnsNotFound()
        {
            int testId = 99;
            var result = await _controller.DeleteCampaignVolunteer(testId);
            Assert.IsType<NotFoundResult>(result);
        }
    }
}