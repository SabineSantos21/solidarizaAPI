using Moq;
using Xunit;
using Solidariza.Controllers;
using Solidariza.Interfaces.Services;
using Solidariza.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Solidariza.Tests
{
    public class DonationControllerTests
    {
        private static ConnectionDB GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ConnectionDB>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ConnectionDB(options);
        }

        private static DonationController CreateController(ConnectionDB dbContext, Mock<IDonationService> donationServiceMock)
        {
            return new DonationController(donationServiceMock.Object);
        }

        [Fact]
        public async Task GetDonationQRCode_ReturnsOkWithOrganizationInfo_WhenFound()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            var donationServiceMock = new Mock<IDonationService>();

            var orgInfo = new OrganizationInfo
            {
                OrganizationInfoId = 1,
                UserId = 1,
                PixKey = "pix@example.com",
                BeneficiaryName = "ONG Teste",
                DonationQRCode = new DonationQRCodeResponse { Qrcode_base64 = "base64string", Name = "PIX" }
            };

            // Configuração sem conversão, pois a interface espera Task<OrganizationInfo>
            donationServiceMock.Setup(s => s.GetQRCodePixByCampaignId(It.IsAny<int>()))
                .ReturnsAsync(orgInfo); // Remove a conversão para OrganizationInfo?

            var controller = CreateController(context, donationServiceMock);

            // Act
            var result = await controller.GetDonationQRCode(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedOrgInfo = Assert.IsType<OrganizationInfo>(okResult.Value);
            Assert.Equal(orgInfo.OrganizationInfoId, returnedOrgInfo.OrganizationInfoId);
            Assert.NotNull(returnedOrgInfo?.DonationQRCode);
            Assert.Equal(orgInfo.DonationQRCode.Qrcode_base64, returnedOrgInfo.DonationQRCode.Qrcode_base64);
        }

        [Fact]
        public async Task GetDonationQRCode_ReturnsProblem_WhenExceptionThrown()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            var donationServiceMock = new Mock<IDonationService>();
            donationServiceMock.Setup(s => s.GetQRCodePixByCampaignId(It.IsAny<int>()))
                .ThrowsAsync(new Exception("Erro inesperado"));

            var controller = new DonationController(donationServiceMock.Object);

            // Act
            var result = await controller.GetDonationQRCode(1);

            // Assert
            var problemResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, problemResult.StatusCode ?? 500);

            var problemDetails = Assert.IsType<ProblemDetails>(problemResult.Value);
            Assert.Equal("Erro inesperado", problemDetails.Detail);
        }
    }
}