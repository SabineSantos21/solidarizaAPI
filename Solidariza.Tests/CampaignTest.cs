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

namespace Solidariza.Tests
{
    public class CampaignControllerTests
    {
        private readonly CampaignController _controller;
        private readonly ConnectionDB _dbContext;

        public CampaignControllerTests()
        {
            var options = new DbContextOptionsBuilder<ConnectionDB>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new ConnectionDB(options);
            _dbContext.Database.EnsureDeleted();
            _dbContext.Database.EnsureCreated();

            SeedDatabase(_dbContext);
            _controller = new CampaignController(_dbContext);
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
            var result = await _controller.GetCampaigns();
            var okResult = Assert.IsType<ActionResult<List<Campaign>>>(result);
            Assert.NotNull(okResult.Value);
            Assert.NotEmpty(okResult.Value);
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
        public async Task GetCampaignById_NonExistingId_ReturnsNotFound()
        {
            var result = await _controller.GetCampaignById(99);
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetCampaignByUserId_ExistingUserId_ReturnsCampaigns()
        {
            var result = await _controller.GetCampaignByUserId(1);
            var okResult = Assert.IsType<ActionResult<List<Campaign>>>(result);
            Assert.NotNull(okResult.Value);
            Assert.NotEmpty(okResult.Value);
        }

        [Fact]
        public async Task GetCampaignByUserId_NonExistingUserId_ReturnsNotFound()
        {
            var result = await _controller.GetCampaignByUserId(999);
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

            var result = await _controller.CreateCampaign(newCampaign);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var createdCampaign = Assert.IsType<Campaign>(okResult.Value);
            Assert.Equal("New Campaign", createdCampaign.Title);
        }

        [Fact]
        public async Task CreateCampaign_WhenExceptionThrown_ReturnsProblem()
        {
            var mockService = new Mock<CampaignService>(_dbContext);
            mockService.Setup(s => s.CreateCampaign(It.IsAny<NewCampaign>()))
                       .ThrowsAsync(new Exception("Erro simulado"));

            var controller = new CampaignController(_dbContext);

            var newCampaign = new NewCampaign
            {
                UserId = 1,
                Title = "Teste Erro",
                Description = "Descrição",
                Type = (int)CampaignType.Collection,
                Status = (int)CampaignStatus.Active
            };

            var result = await controller.CreateCampaign(newCampaign);

            var problemResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, problemResult.StatusCode);
        }

        [Fact]
        public async Task PutCampaign_ExistingId_UpdatesCampaign()
        {
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

            var result = await _controller.PutCampaign(1, updateCampaign);
            Assert.IsType<NoContentResult>(result);

            var updated = await _dbContext.Campaign.FindAsync(1);
            Assert.Equal("Updated Campaign", updated.Title);
            Assert.Equal(CampaignStatus.Completed, updated.Status);
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
