using Solidariza.Models;
using Microsoft.EntityFrameworkCore;
using Solidariza.Models.Enum;
using System.Text.Json;
using System.Net.Http;
using System.Text;

namespace Solidariza.Services
{
    public class DonationService
    {
        private readonly ConnectionDB _dbContext;
        private readonly HttpClient _httpClient;

        public DonationService(ConnectionDB dbContext) 
        {
            _dbContext = dbContext;
            _httpClient = new HttpClient();
        }

        public async Task<OrganizationInfo> GetQRCodePixByCampaignId(int campaignId)
        {
            try
            {
                CampaignService campaignService =  new CampaignService(_dbContext);
                Campaign campaign = await campaignService.GetCampaignById(campaignId);

                if(campaign == null) 
                    throw new Exception("Não foi possível localizar a campanha");

                OrganizationInfoService organizationInfoService = new OrganizationInfoService(_dbContext);
                OrganizationInfo organizationInfo = await organizationInfoService.GetOrganizationInfoByUserId(campaign.UserId);

                if (campaign == null)
                    throw new Exception("Não foi possível localizar a as informações da organização");

                DonationQRCodeRequest donationQRCodeRequest = new DonationQRCodeRequest()
                {
                    Key = organizationInfo.PixKey,
                    Key_type = organizationInfo.PixType.ToString(),
                    Name = organizationInfo.BeneficiaryName,
                    City = organizationInfo.BeneficiaryCity
                };

                var pixQrCodeJson = GetDonationQRCode(donationQRCodeRequest);
                var pixQrCode = JsonSerializer.Deserialize<DonationQRCodeResponse>(await pixQrCodeJson);

                organizationInfo.DonationQRCode = pixQrCode;

                return organizationInfo;
            }
            catch (Exception ex)
            {
                throw ex;
            }
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

                HttpResponseMessage response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                string responseContent = await response.Content.ReadAsStringAsync();

                return responseContent;
            }
            catch (Exception ex)
            {
                return $"Erro: {ex.Message}";
            }
        }
    }
}
