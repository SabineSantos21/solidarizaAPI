using Solidariza.Models;
using Microsoft.EntityFrameworkCore;
using Solidariza.Models.Enum;

namespace Solidariza.Services
{
    public class OrganizationInfoService
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
            try
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
            catch (Exception ex)
            {
                throw ex;
            }
         
        }

        public async Task<OrganizationInfo> CreateOrganizationInfoCPNJValid(NewOrganizationInfoCNPJValid newOrganizationInfo)
        {
            try
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
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task AtualizarOrganizationInfo(OrganizationInfo existingOrganizationInfo, OrganizationInfo OrganizationInfo)
        {
            try
            {
                existingOrganizationInfo.PixKey = OrganizationInfo.PixKey;
                existingOrganizationInfo.ContactPhone = existingOrganizationInfo.ContactPhone;
                existingOrganizationInfo.ContactName = existingOrganizationInfo.ContactName;
                existingOrganizationInfo.PixType = existingOrganizationInfo.PixType;
                existingOrganizationInfo.BeneficiaryName = existingOrganizationInfo.BeneficiaryName;
                existingOrganizationInfo.BeneficiaryCity = existingOrganizationInfo.BeneficiaryCity;

                _dbContext.Organization_Info.Update(existingOrganizationInfo);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<OrganizationInfo> AtualizarOrganizationInfoValidate(OrganizationInfo existingOrganizationInfo, OrganizationInfo OrganizationInfo)
        {
            try
            {
                existingOrganizationInfo.DisapprovalReason = OrganizationInfo.DisapprovalReason;
                existingOrganizationInfo.IsOrganizationApproved = existingOrganizationInfo.IsOrganizationApproved;

                _dbContext.Organization_Info.Update(existingOrganizationInfo);
                await _dbContext.SaveChangesAsync();

                return existingOrganizationInfo;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task DeleteOrganizationInfo(OrganizationInfo organizationInfo)
        {
            _dbContext.Organization_Info.Remove(organizationInfo);
            await _dbContext.SaveChangesAsync();
        }
    }
}
