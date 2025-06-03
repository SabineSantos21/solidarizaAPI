using Solidariza.Models;
using System.Text.Json;
using System.Text;
using Solidariza.Interfaces.Services;

namespace Solidariza.Services
{
    public class DonationService: IDonationService
    {
        private readonly ConnectionDB _dbContext;
        private readonly HttpClient _httpClient;
        private readonly ICampaignService _campaignService;
        private readonly IOrganizationInfoService _organizationInfoService;

        public DonationService(ConnectionDB dbContext, ICampaignService campaignService, IOrganizationInfoService organizationInfoService) 
        {
            _dbContext = dbContext;
            _httpClient = new HttpClient();
            _campaignService = campaignService;
            _organizationInfoService = organizationInfoService;
        }

        public async Task<OrganizationInfo> GetQRCodePixByCampaignId(int campaignId)
        {
            var campaign = await _campaignService.GetCampaignById(campaignId);
            if (campaign == null)
                throw new InvalidOperationException("Não foi possível localizar a campanha");

            var organizationInfo = await _organizationInfoService.GetOrganizationInfoByUserId(campaign.UserId);
            if (organizationInfo == null)
                throw new InvalidOperationException("Não foi possível localizar as informações da organização");

            var donationQRCodeRequest = new DonationQRCodeRequest
            {
                Key = organizationInfo.PixKey,
                Key_type = organizationInfo.PixType.ToString(),
                Name = organizationInfo.BeneficiaryName,
                City = organizationInfo.BeneficiaryCity
            };

            var pixQrCodeJson = await GetDonationQRCode(donationQRCodeRequest);
            var pixQrCode = JsonSerializer.Deserialize<DonationQRCodeResponse>(pixQrCodeJson);
            organizationInfo.DonationQRCode = pixQrCode;

            return organizationInfo;
        }

        public async Task<string> GetDonationQRCode(DonationQRCodeRequest donationQRCodeRequest)
        {
            string url = "https://www.gerarpix.com.br/emvqr-static";
            string jsonPayload = JsonSerializer.Serialize(donationQRCodeRequest);

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json")
                };

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                return $"Erro: {ex.Message}";
            }
        }
    }
}
