using Microsoft.EntityFrameworkCore;
using Xunit;
using Moq;
using Solidariza.Controllers;
using Solidariza.Models;
using Solidariza.Services;
using Microsoft.AspNetCore.Mvc;
using Solidariza.Models.Enum;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.VisualStudio.TestPlatform.CrossPlatEngine;

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

            SeedDatabase(_dbContext);

            _controller = new DonationController(_dbContext);
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

                dbContext.Organization_Info.Add(new OrganizationInfo
                {
                    UserId = 1,
                    ContactName = "Contato Teste",
                    ContactPhone = "47983758492",
                    PixKey = "11111111111",
                    PixType = PixType.CPF,
                    BeneficiaryName = "PIX Teste",
                    BeneficiaryCity = "Joinville",
                });

                dbContext.SaveChanges();
            }
        }

        [Fact]
        public async Task GetDonationQRCode_ExistingCampaignId_ReturnsOrganizationInfo()
        {
            int campaignId = 1; // Assumindo que esse ID existe no banco de dados de teste
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