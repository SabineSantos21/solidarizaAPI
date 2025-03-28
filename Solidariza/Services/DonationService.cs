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
