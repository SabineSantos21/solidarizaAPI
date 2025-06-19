using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Solidariza.Controllers;
using Solidariza.Models;
using Solidariza.Services;
using Solidariza.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Solidariza.Interfaces.Services;

namespace Solidariza.Tests
{
    public class CampaignControllerTests
    {
        private readonly CampaignController _controller;
        private readonly ConnectionDB _dbContext;
        public Mock<ICampaignService> _campaignService { get; set; }

        public CampaignControllerTests()
        {
            var options = new DbContextOptionsBuilder<ConnectionDB>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new ConnectionDB(options);
            _dbContext.Database.EnsureDeleted();
            _dbContext.Database.EnsureCreated();

            SeedDatabase(_dbContext);

            _campaignService = new Mock<ICampaignService>();
            _controller = new CampaignController(_dbContext, _campaignService.Object);
        }

        private static void SeedDatabase(ConnectionDB dbContext)
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
                    Status = CampaignStatus.Active,
                    State = "SP",
                    City = "São Paulo",
                    Address = "Rua A"
                });
                dbContext.SaveChanges();
            }
        }

        [Fact]
        public async Task GetCampaigns_ReturnsListOfCampaigns()
        {
            var campaigns = new List<Campaign>
            {
                new Campaign { CampaignId = 1, Title = "Campanha 1" },
                new Campaign { CampaignId = 2, Title = "Campanha 2" }
            };

            _campaignService.Setup(s => s.GetCampaigns())
                .ReturnsAsync(campaigns);

            var result = await _controller.GetCampaigns();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedCampaigns = Assert.IsType<List<Campaign>>(okResult.Value);

            Assert.NotNull(returnedCampaigns);
            Assert.Equal(2, returnedCampaigns.Count);
        }

        [Fact]
        public async Task GetCampaigns_WhenEmpty_ReturnsNotFound()
        {
            _dbContext.Campaign.RemoveRange(_dbContext.Campaign);
            await _dbContext.SaveChangesAsync();

            var result = await _controller.GetCampaigns();
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetCampaigns_ReturnsNotFound_WhenServiceReturnsNull()
        {
            _campaignService.Setup(s => s.GetCampaigns())
                .ReturnsAsync((List<Campaign>?)null);

            var result = await _controller.GetCampaigns();

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetCampaignById_ReturnsCampaign_WhenExists()
        {
            var campaign = new Campaign { CampaignId = 10, Title = "Campanha Teste" };

            _campaignService.Setup(s => s.GetCampaignById(10))
                .ReturnsAsync(campaign);

            var result = await _controller.GetCampaignById(10);

            var okResult = Assert.IsType<ActionResult<Campaign>>(result);
            var returnedCampaign = Assert.IsType<Campaign>(okResult.Value);

            Assert.NotNull(returnedCampaign);
            Assert.Equal("Campanha Teste", returnedCampaign.Title);
        }

        [Fact]
        public async Task GetCampaignById_NonExistingId_ReturnsNotFound()
        {
            var result = await _controller.GetCampaignById(99);
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetCampaignById_ReturnsNotFound_WhenServiceReturnsNull()
        {
            _campaignService.Setup(s => s.GetCampaignById(It.IsAny<int>()))
                .ReturnsAsync((Campaign?)null);

            var result = await _controller.GetCampaignById(999);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetCampaignByUserId_ExistingUserId_ReturnsCampaigns()
        {
            var userId = 1;
            var campaigns = new List<Campaign>
            {
                new Campaign { CampaignId = 1, Title = "Campanha 1", UserId = userId },
                new Campaign { CampaignId = 2, Title = "Campanha 2", UserId = userId }
            };

            _campaignService.Setup(s => s.GetCampaignByUserId(userId))
                .ReturnsAsync(campaigns);

            var result = await _controller.GetCampaignByUserId(userId);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedCampaigns = Assert.IsType<List<Campaign>>(okResult.Value);

            Assert.NotNull(returnedCampaigns);
            Assert.Equal(2, returnedCampaigns.Count);
            Assert.All(returnedCampaigns, c => Assert.Equal(userId, c.UserId));
        }

        [Fact]
        public async Task GetCampaignByUserId_ReturnsNotFound_WhenServiceReturnsNull()
        {
            _campaignService.Setup(s => s.GetCampaignByUserId(It.IsAny<int>()))
                .ReturnsAsync((List<Campaign>?)null);

            var result = await _controller.GetCampaignByUserId(42);

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
                Status = (int)CampaignStatus.Active,
                State = "SC",
                City = "Joinville",
                Address = "Av. Central",
                StartDate = DateTime.Now.ToString(),
                EndDate = DateTime.Now.AddDays(5).ToString(),
            };

            var expectedCampaign = new Campaign
            {
                CampaignId = 123, // ou qualquer outro valor necessário
                UserId = 1,
                Title = "New Campaign",
                Description = "New Description",
                Type = CampaignType.Collection,
                Status = CampaignStatus.Active,
                State = "SC",
                City = "Joinville",
                Address = "Av. Central",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(5)
            };

            _campaignService.Setup(s => s.CreateCampaign(It.IsAny<NewCampaign>()))
            .ReturnsAsync(expectedCampaign);

            var result = await _controller.CreateCampaign(newCampaign);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var createdCampaign = Assert.IsType<Campaign>(okResult.Value);

            Assert.Equal("New Campaign", createdCampaign.Title);
        }

        [Fact]
        public async Task CreateCampaign_WhenExceptionThrown_ReturnsProblem()
        {
            _campaignService.Setup(s => s.CreateCampaign(It.IsAny<NewCampaign>()))
                       .ThrowsAsync(new Exception("Erro simulado"));

            var newCampaign = new NewCampaign
            {
                UserId = 1,
                Title = "Teste Erro",
                Description = "Descrição",
                Type = (int)CampaignType.Collection,
                Status = (int)CampaignStatus.Active
            };

            var result = await _controller.CreateCampaign(newCampaign);

            var problemResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, problemResult.StatusCode);
        }

        [Fact]
        public async Task PutCampaign_NonExistingId_ReturnsNotFound()
        {
            var updateCampaign = new UpdateCampaign
            {
                Title = "Updated",
                Description = "Desc",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(10),
                Type = (int)CampaignType.Collection,
                Status = (int)CampaignStatus.Active,
                State = "PR",
                City = "Curitiba",
                Address = "Rua X"
            };

            var result = await _controller.PutCampaign(999, updateCampaign);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteCampaign_ExistingId_DeletesCampaign()
        {
            var result = await _controller.DeleteCampaign(1);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteCampaign_NonExistingId_ReturnsNotFound()
        {
            var result = await _controller.DeleteCampaign(999);
            Assert.IsType<NotFoundResult>(result);
        }
    }
}