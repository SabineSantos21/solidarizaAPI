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

            ApicnpjConsultResponseObject organizationResponseAPI = await GetOrganizationByAPI(cnpj);

            if (organizationResponseAPI.IsSuccess == false)
            {
                var organizationError = JsonSerializer.Deserialize<ApicnpjConsultResponseError>(organizationResponseAPI.Response);

                return new ConsultCnpjResponse()
                {
                    DisapprovalReason = organizationError?.Detalhes,
                    IsValid = false
                };
            }

            var organization = JsonSerializer.Deserialize<ApicnpjConsultResponse>(organizationResponseAPI.Response);

            if (organization?.CnpjRaiz == null)
            {
                return new ConsultCnpjResponse()
                {
                    DisapprovalReason = "Não encontramos essa empresa. Verifique se o CNPJ está correto e tente novamente",
                    IsValid = false
                };
            }

            bool isAtiva = organization.Estabelecimento?.SituacaoCadastral == "Ativa";

            if (!isAtiva) {
                    
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

            if (!isSemFinsLucrativos) {
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

        public async Task<ApicnpjConsultResponseObject> GetOrganizationByAPI(string cnpj)
        {
            string url = $"https://publica.cnpj.ws/cnpj/{cnpj}";
            ApicnpjConsultResponseObject responseApi = new ApicnpjConsultResponseObject();

            try
            {

                var request = new HttpRequestMessage(HttpMethod.Get, url);

                HttpResponseMessage response = await _httpClient.SendAsync(request);

                var responseContent = string.Empty;

                if (response.IsSuccessStatusCode)
                {
                    responseApi.IsSuccess = true;
                }
                else
                {
                    responseApi.IsSuccess = false;
                }
                
                responseApi.Response = await response.Content.ReadAsStringAsync();

                return responseApi;
            }
            catch (Exception ex)
            {
                responseApi.IsSuccess = false;
                responseApi.Response = $"Erro: {ex.Message}";

                return responseApi;
            }
        }
    }
}
