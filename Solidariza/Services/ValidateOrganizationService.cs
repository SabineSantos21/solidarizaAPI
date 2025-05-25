using Solidariza.Interfaces.Services;
using Solidariza.Models;
using System.Text.Json;

namespace Solidariza.Services
{
    public class ValidateOrganizationService: IValidateOrganizationService
    {
        private readonly HttpClient _httpClient;

        public ValidateOrganizationService() 
        {
            _httpClient = new HttpClient();
        }

        public async Task<ConsultCnpjResponse> ConsultCNPJ(string cnpj)
        {

            var organizationJson = GetOrganizationByAPI(cnpj);
            var organization = JsonSerializer.Deserialize<ApicnpjConsultResponse>(await organizationJson);

            if (organization == null)
            {
                return new ConsultCnpjResponse()
                {
                    DisapprovalReason = "Não encontramos essa empresa. Verifique se o CNPJ está correto e tente novamente",
                    IsValid = false
                };
            }

            bool isAtiva = organization.Estabelecimento?.SituacaoCadastral == "Ativa";

            if (isAtiva == false) {
                    
                return new ConsultCnpjResponse()
                {
                    DisapprovalReason = "Este CNPJ está inativo no momento. Confira a situação cadastral antes de prosseguir.",
                    IsValid = false
                };
            }

            int[] NaturezasPermitidas = [3999, 3069, 3220, 2143, 2232, 2281, 4083];

            if (organization.NaturezaJuridica == null || organization.NaturezaJuridica.Id == null)
            {
                return new ConsultCnpjResponse()
                {
                    DisapprovalReason = "Empresa não possui Natureza Jurídica",
                    IsValid = false
                };
            }

            bool isSemFinsLucrativos = organization.NaturezaJuridica != null &&
                                        NaturezasPermitidas.Contains(int.Parse(organization.NaturezaJuridica.Id));

            if (isSemFinsLucrativos == false) {
                return new ConsultCnpjResponse()
                {
                    DisapprovalReason = "Esta organização não é registrada como sem fins lucrativos. Confira as informações e tente novamente.",
                    IsValid = false
                };
            }

            return new ConsultCnpjResponse()
            {
                DisapprovalReason = "",
                IsValid = true
            };
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
