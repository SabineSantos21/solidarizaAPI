using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Solidariza.Controllers;
using Solidariza.Models;
using Solidariza.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Solidariza.Models.Enum;
using System;

namespace Solidariza.Tests
{
    public class CampaignControllerTests
    {
        private readonly CampaignController _controller;
        private readonly ConnectionDB _dbContext;
        private readonly Mock<CampaignService> _mockCampaignService;

        public CampaignControllerTests()
        {
            var options = new DbContextOptionsBuilder<ConnectionDB>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new ConnectionDB(options);
            _mockCampaignService = new Mock<CampaignService>(_dbContext);

            _dbContext.Database.EnsureDeleted();
            _dbContext.Database.EnsureCreated();

            SeedDatabase(_dbContext);

            _controller = new CampaignController(_dbContext);
        }

        private void SeedDatabase(ConnectionDB dbContext)
        {
            if (!dbContext.Campaign.Any())
            {
                dbContext.Campaign.Add(new Campaign
                {
                    CampaignId = 1,
                    Title = "Test Campaign",
                    UserId = 1,
                    Description = "Description",
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(10),
                    Type = CampaignType.Collection,
                    Status = CampaignStatus.Active
                });

                dbContext.SaveChanges();
            }
        }

        [Fact]
        public async Task GetCampaigns_ReturnsListOfCampaigns()
        {
            var result = await _controller.GetCampaigns();
            var okResult = Assert.IsType<ActionResult<List<Campaign>>>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task GetCampaignById_NonExistingId_ReturnsNotFound()
        {
            int testCampaignId = 99;
            var result = await _controller.GetCampaignById(testCampaignId);
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetCampaignByUserId_ExistingUserId_ReturnsCampaigns()
        {
            int testUserId = 1;
            var result = await _controller.GetCampaignByUserId(testUserId);
            var okResult = Assert.IsType<ActionResult<List<Campaign>>>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task CreateCampaign_ValidCampaign_ReturnsOk()
        {
            var newCampaign = new NewCampaign
            {
                UserId = 1,
                Title = "New Campaign",
                Description = "New Description",
                Type = (int)CampaignType.Collection,
                Status = (int)CampaignStatus.Active
            };

            var result = await _controller.CreateCampaign(newCampaign);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var createdCampaign = Assert.IsType<Campaign>(okResult.Value);
            Assert.Equal("New Campaign", createdCampaign.Title);
        }

        [Fact]
        public async Task PutCampaign_ExistingId_UpdatesCampaign()
        {
            int testCampaignId = 1;
            var updateCampaign = new UpdateCampaign
            {
                Title = "Updated Campaign",
                Description = "Updated Description",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(15),
                Type = (int)CampaignType.Collection,
                Status = (int)CampaignStatus.Completed,
                State = "SC",
                City = "Joinville",
                Address = "New Address"
            };

            var result = await _controller.PutCampaign(testCampaignId, updateCampaign);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task PutCampaign_NonExistingId_ReturnsNotFound()
        {
            int testCampaignId = 99;
            var updateCampaign = new UpdateCampaign
            {
                Title = "Updated Campaign",
                Description = "Updated Description",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(15),
                Type = (int)CampaignType.Collection,
                Status = (int)CampaignStatus.Completed,
                State = "SC",
                City = "Joinville",
                Address = "New Address"
            };

            var result = await _controller.PutCampaign(testCampaignId, updateCampaign);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteCampaign_ExistingId_DeletesCampaign()
        {
            int testCampaignId = 1;
            var result = await _controller.DeleteCampaign(testCampaignId);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteCampaign_NonExistingId_ReturnsNotFound()
        {
            int testCampaignId = 99;
            var result = await _controller.DeleteCampaign(testCampaignId);
            Assert.IsType<NotFoundResult>(result);
        }
    }
}