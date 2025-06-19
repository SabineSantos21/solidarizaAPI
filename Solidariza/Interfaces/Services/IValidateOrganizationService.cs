using Solidariza.Models;

namespace Solidariza.Interfaces.Services
{
    public interface IValidateOrganizationService
    {
        Task<ConsultCnpjResponse> ConsultCNPJ(string cnpj);

        Task<ApicnpjConsultResponseObject> GetOrganizationByAPI(string cnpj);
    }
}
