using Solidariza.Models;

namespace Solidariza.Interfaces.Services
{
    public interface IOrganizationInfoService
    {
        Task<OrganizationInfo?> GetOrganizationInfoById(int id);
        
        Task<OrganizationInfo?> GetOrganizationInfoByUserId(int userId);
        
        Task<OrganizationInfo> CreateOrganizationInfo(NewOrganizationInfo newOrganizationInfo);
        
        Task<OrganizationInfo> CreateOrganizationInfoCPNJValid(NewOrganizationInfoCnpjValid newOrganizationInfo);
        
        Task AtualizarOrganizationInfo(OrganizationInfo existingOrganizationInfo, OrganizationInfo organizationInfo);
        
        Task<OrganizationInfo> AtualizarOrganizationInfoValidate(OrganizationInfo existingOrganizationInfo, OrganizationInfo organizationInfo);
        
        Task DeleteOrganizationInfo(OrganizationInfo organizationInfo);
    }
}
