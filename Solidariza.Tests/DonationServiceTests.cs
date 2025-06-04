using Moq;
using Moq.Protected;
using System.Net;
using System.Text;
using System.Text.Json;
using Solidariza.Models;
using Solidariza.Services;
using Solidariza.Interfaces.Services;
using Microsoft.EntityFrameworkCore;

namespace Solidariza.Tests
{
    public class DonationServiceTests
    {
        private static ConnectionDB GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ConnectionDB>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ConnectionDB(options);
        }

        private static HttpClient GetMockHttpClient(string responseContent)
        {
            var handlerMock = new Mock<HttpMessageHandler>();

            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                   "SendAsync",
                   ItExpr.IsAny<HttpRequestMessage>(),
                   ItExpr.IsAny<CancellationToken>()
               )
               .ReturnsAsync(new HttpResponseMessage
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent(responseContent, Encoding.UTF8, "application/json")
               });

            return new HttpClient(handlerMock.Object);
        }

        [Fact]
        public async Task GetQRCodePixByCampaignId_ReturnsQRCode()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            var campaign = new Campaign { CampaignId = 1, UserId = 1 };
            var orgInfo = new OrganizationInfo
            {
                OrganizationInfoId = 1,
                UserId = 1,
                PixKey = "pix@example.com",
                PixType = Models.Enum.PixType.EMAIL,
                BeneficiaryName = "ONG Teste",
                BeneficiaryCity = "Cidade"
            };

            context.Campaign.Add(campaign);
            context.Organization_Info.Add(orgInfo);
            await context.SaveChangesAsync();

            var expectedQRCode = new DonationQRCodeResponse
            {
                Qrcode_base64 = "qrcode123",
                Name = "PIX COPY/PASTE"
            };

            var campaignServiceMock = new Mock<ICampaignService>();
            campaignServiceMock.Setup(s => s.GetCampaignById(1))
                .ReturnsAsync(campaign);

            var orgInfoServiceMock = new Mock<IOrganizationInfoService>();
            orgInfoServiceMock.Setup(s => s.GetOrganizationInfoByUserId(1))
                .ReturnsAsync(orgInfo);

            var mockedHttpClient = GetMockHttpClient(JsonSerializer.Serialize(expectedQRCode));

            DonationService service = new DonationService(campaignServiceMock.Object, orgInfoServiceMock.Object);

            // Injeta HttpClient mockado via reflexão
            var field = typeof(DonationService)
                .GetField("_httpClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.NotNull(field);
            field.SetValue(service, mockedHttpClient);

            // Act
            var result = await service.GetQRCodePixByCampaignId(1);

            // Assert
            Assert.NotNull(result.DonationQRCode);
            Assert.Equal(expectedQRCode.Qrcode_base64, result.DonationQRCode.Qrcode_base64);
        }

        [Fact]
        public async Task GetQRCodePixByCampaignId_CampaignNotFound_ThrowsException()
        {
            var context = GetInMemoryDbContext();

            var campaignServiceMock = new Mock<ICampaignService>();
            campaignServiceMock.Setup(s => s.GetCampaignById(It.IsAny<int>()))
                .ReturnsAsync((Campaign?)null);

            var orgInfoServiceMock = new Mock<IOrganizationInfoService>();

            DonationService service = new DonationService(campaignServiceMock.Object, orgInfoServiceMock.Object);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                service.GetQRCodePixByCampaignId(99));

            Assert.Equal("Não foi possível localizar a campanha", ex.Message);
        }

        [Fact]
        public async Task GetQRCodePixByCampaignId_OrgInfoNotFound_ThrowsException()
        {
            var context = GetInMemoryDbContext();

            var campaign = new Campaign { CampaignId = 1, UserId = 1 };
            var campaignServiceMock = new Mock<ICampaignService>();
            campaignServiceMock.Setup(s => s.GetCampaignById(1))
                .ReturnsAsync(campaign);

            var orgInfoServiceMock = new Mock<IOrganizationInfoService>();
            orgInfoServiceMock.Setup(s => s.GetOrganizationInfoByUserId(1))
                .ReturnsAsync((OrganizationInfo?)null);

            DonationService service = new DonationService(campaignServiceMock.Object, orgInfoServiceMock.Object);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                service.GetQRCodePixByCampaignId(1));

            Assert.Equal("Não foi possível localizar as informações da organização", ex.Message);
        }

        [Fact]
        public async Task GetDonationQRCode_ReturnsExpectedJson()
        {
            var expected = new DonationQRCodeResponse { Qrcode_base64 = "123abc", Name = "copypaste" };
            var client = GetMockHttpClient(JsonSerializer.Serialize(expected));
            var context = GetInMemoryDbContext();

            var campaignServiceMock = new Mock<ICampaignService>();
            var orgInfoServiceMock = new Mock<IOrganizationInfoService>();

            DonationService service = new DonationService(campaignServiceMock.Object, orgInfoServiceMock.Object);

            var field = typeof(DonationService)
                .GetField("_httpClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.NotNull(field);
            field.SetValue(service, client);

            var request = new DonationQRCodeRequest
            {
                Key = "pix@example.com",
                Key_type = "EMAIL",
                Name = "ONG",
                City = "Cidade"
            };

            var json = await service.GetDonationQRCode(request);
            var response = JsonSerializer.Deserialize<DonationQRCodeResponse>(json);

            Assert.Equal(expected.Qrcode_base64, response?.Qrcode_base64);
            Assert.Equal(expected.Name, response?.Name);
        }

        [Fact]
        public async Task GetDonationQRCode_OnHttpFailure_ReturnsErrorMessage()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                   "SendAsync",
                   ItExpr.IsAny<HttpRequestMessage>(),
                   ItExpr.IsAny<CancellationToken>()
               )
               .ThrowsAsync(new HttpRequestException("Erro de rede"));

            var client = new HttpClient(handlerMock.Object);
            var context = GetInMemoryDbContext();

            var campaignServiceMock = new Mock<ICampaignService>();
            var orgInfoServiceMock = new Mock<IOrganizationInfoService>();

            DonationService service = new DonationService(campaignServiceMock.Object, orgInfoServiceMock.Object);

            var field = typeof(DonationService)
                .GetField("_httpClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.NotNull(field);
            field.SetValue(service, client);

            var result = await service.GetDonationQRCode(new DonationQRCodeRequest());

            Assert.Contains("Erro:", result);
        }
    }
}