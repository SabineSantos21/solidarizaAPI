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
        public async Task GetCampaignById_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            int testCampaignId = 99;

            // Act
            var result = await _controller.GetCampaignById(testCampaignId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
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

    }
}