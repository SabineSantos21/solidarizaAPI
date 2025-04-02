using Solidariza.Models;
using System.Text.Json;

namespace Solidariza.Services
{
    public class ValidateOrganizationService
    {
        private readonly ConnectionDB _dbContext;
        private readonly HttpClient _httpClient;

        public ValidateOrganizationService(ConnectionDB dbContext) 
        {
            _dbContext = dbContext;
            _httpClient = new HttpClient();
        }

        public async Task<ConsultCNPJResponse> ConsultCNPJ(string cnpj)
        {
            try
            {
                var organizationJson = GetOrganizationByAPI(cnpj);
                var organization = JsonSerializer.Deserialize<APICNPJConsultResponse>(await organizationJson);

                if (organization == null)
                {
                    return new ConsultCNPJResponse()
                    {
                        DisapprovalReason = "Não encontramos essa empresa. Verifique se o CNPJ está correto e tente novamente",
                        IsValid = false
                    };
                }

                bool isAtiva = organization.Estabelecimento?.SituacaoCadastral == "Ativa";

                if (isAtiva == false) {
                    
                    return new ConsultCNPJResponse()
                    {
                        DisapprovalReason = "Este CNPJ está inativo no momento. Confira a situação cadastral antes de prosseguir.",
                        IsValid = false
                    };
                }

                bool isSemFinsLucrativos = organization.NaturezaJuridica != null &&
                                           new[] { 3999, 3069, 3220, 2143, 2232, 2281, 4083 }
                                           .Contains(int.Parse(organization.NaturezaJuridica.Id));

                if (isSemFinsLucrativos == false) {
                    return new ConsultCNPJResponse()
                    {
                        DisapprovalReason = "Esta organização não é registrada como sem fins lucrativos. Confira as informações e tente novamente.",
                        IsValid = false
                    };
                }

                return new ConsultCNPJResponse()
                {
                    DisapprovalReason = "",
                    IsValid = true
                };

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> GetOrganizationByAPI(string cnpj)
        {
            string url = $"https://publica.cnpj.ws/cnpj/{cnpj}";

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, url);

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
