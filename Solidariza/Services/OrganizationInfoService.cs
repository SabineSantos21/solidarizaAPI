using Solidariza.Models;
using Microsoft.EntityFrameworkCore;
using Solidariza.Models.Enum;
using Solidariza.Interfaces.Services;

namespace Solidariza.Services
{
    public class OrganizationInfoService: IOrganizationInfoService
    {
        private readonly ConnectionDB _dbContext;

        public OrganizationInfoService(ConnectionDB dbContext) 
        {
            _dbContext = dbContext;
        }

        public async Task<OrganizationInfo?> GetOrganizationInfoById(int id)
        {
            return await _dbContext.Organization_Info.FirstOrDefaultAsync(p => p.OrganizationInfoId == id);
        }

        public async Task<OrganizationInfo?> GetOrganizationInfoByUserId(int userId)
        {
            return await _dbContext.Organization_Info.Include(o => o.User).FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task<OrganizationInfo> CreateOrganizationInfo(NewOrganizationInfo newOrganizationInfo)
        {

            OrganizationInfo organizationInfo = new OrganizationInfo()
            {
                UserId = newOrganizationInfo.UserId,
                ContactName = newOrganizationInfo.ContactName,
                ContactPhone = newOrganizationInfo.ContactPhone,
                PixKey = newOrganizationInfo.PixKey,
                PixType = (PixType) newOrganizationInfo.PixType,
                BeneficiaryName = newOrganizationInfo.BeneficiaryName,
                BeneficiaryCity = newOrganizationInfo.BeneficiaryCity,
                PixValue = newOrganizationInfo.PixValue,
            };

            _dbContext.Organization_Info.Add(organizationInfo);
            await _dbContext.SaveChangesAsync();

            return organizationInfo;
         
        }

        public async Task<OrganizationInfo> CreateOrganizationInfoCPNJValid(NewOrganizationInfoCnpjValid newOrganizationInfo)
        {

            OrganizationInfo organizationInfo = new OrganizationInfo()
            {
                UserId = newOrganizationInfo.UserId,
                IsOrganizationApproved = newOrganizationInfo.IsOrganizationApproved,
                DisapprovalReason = newOrganizationInfo.DisapprovalReason
            };

            _dbContext.Organization_Info.Add(organizationInfo);
            await _dbContext.SaveChangesAsync();

            return organizationInfo;

        }

        public async Task AtualizarOrganizationInfo(OrganizationInfo existingOrganizationInfo, OrganizationInfo organizationInfo)
        {

            existingOrganizationInfo.PixKey = organizationInfo.PixKey;
            existingOrganizationInfo.ContactPhone = organizationInfo.ContactPhone;
            existingOrganizationInfo.ContactName = organizationInfo.ContactName;
            existingOrganizationInfo.PixType = organizationInfo.PixType;
            existingOrganizationInfo.BeneficiaryName = organizationInfo.BeneficiaryName;
            existingOrganizationInfo.BeneficiaryCity = organizationInfo.BeneficiaryCity;

            _dbContext.Organization_Info.Update(existingOrganizationInfo);
            await _dbContext.SaveChangesAsync();

        }

        public async Task<OrganizationInfo> AtualizarOrganizationInfoValidate(OrganizationInfo existingOrganizationInfo, OrganizationInfo organizationInfo)
        {

            existingOrganizationInfo.DisapprovalReason = organizationInfo.DisapprovalReason;
            existingOrganizationInfo.IsOrganizationApproved = organizationInfo.IsOrganizationApproved;

            _dbContext.Organization_Info.Update(existingOrganizationInfo);
            await _dbContext.SaveChangesAsync();

            return existingOrganizationInfo;
          
        }

        public async Task DeleteOrganizationInfo(OrganizationInfo organizationInfo)
        {
            _dbContext.Organization_Info.Remove(organizationInfo);
            await _dbContext.SaveChangesAsync();
        }
    }
}
