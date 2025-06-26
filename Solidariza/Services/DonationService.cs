using Solidariza.Models;
using System.Text.Json;
using System.Text;
using Solidariza.Interfaces.Services;

namespace Solidariza.Services
{
    public class DonationService: IDonationService
    {
        private readonly HttpClient _httpClient;
        private readonly ICampaignService _campaignService;
        private readonly IOrganizationInfoService _organizationInfoService;
        private readonly IConfiguration _configuration;

        public DonationService(IConfiguration configuration, ICampaignService campaignService, IOrganizationInfoService organizationInfoService) 
        {
            _httpClient = new HttpClient();
            _campaignService = campaignService;
            _organizationInfoService = organizationInfoService;
            _configuration = configuration;
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

            if (!pixQrCodeJson.TrimStart().StartsWith("{"))
            {
                throw new InvalidOperationException($"Resposta inesperada da API de QR Code: {pixQrCodeJson}");
            }

            var pixQrCode = JsonSerializer.Deserialize<DonationQRCodeResponse>(pixQrCodeJson);
            organizationInfo.DonationQRCode = pixQrCode;

            return organizationInfo;
        }

        public async Task<string> GetDonationQRCode(DonationQRCodeRequest donationQRCodeRequest)
        {
            string? url = _configuration["Pix:QRCodeUrl"];
            string jsonPayload = JsonSerializer.Serialize(donationQRCodeRequest);

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json")
                };

                var response = await _httpClient.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Erro ao gerar QR Code Pix: {response.StatusCode} - {content}");
                }

                return content;
            }
            catch (Exception ex)
            {
                return $"Erro: {ex.Message}";
            }
        }
    }
}
