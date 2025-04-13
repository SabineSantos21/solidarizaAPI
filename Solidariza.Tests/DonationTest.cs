using Microsoft.EntityFrameworkCore;
using Xunit;
using Moq;
using Solidariza.Controllers;
using Solidariza.Models;
using Solidariza.Services;
using Microsoft.AspNetCore.Mvc;
using Solidariza.Models.Enum;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Solidariza.Tests
{
    public class DonationControllerTests
    {
        private readonly DonationController _controller;
        private readonly Mock<DonationService> _mockDonationService;
        private readonly ConnectionDB _dbContext;

        public DonationControllerTests()
        {
            var options = new DbContextOptionsBuilder<ConnectionDB>()
               .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
               .Options;

            _dbContext = new ConnectionDB(options);

            _dbContext.Database.EnsureDeleted();
            _dbContext.Database.EnsureCreated();

            _controller = new DonationController(_dbContext);
        }

        [Fact]
        public async Task GetDonationQRCode_ExistingCampaignId_ReturnsOrganizationInfo()
        {
            int campaignId = 6; // Assumindo que esse ID existe no banco de dados de teste
            var result = await _controller.GetDonationQRCode(campaignId);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.NotNull(objectResult.Value);
            Assert.IsType<OrganizationInfo>(objectResult.Value);
        }

        [Fact]
        public async Task GetDonationQRCode_NonExistingCampaignId_ReturnsNotFound()
        {
            int campaignId = 99; // ID que não existe
            var result = await _controller.GetDonationQRCode(campaignId);
            Assert.IsType<ProblemHttpResult>(result);
        }

    }
}