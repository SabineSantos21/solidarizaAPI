using Solidariza;
using Solidariza.Services;
using Microsoft.AspNetCore.Mvc;
using Solidariza.Models;
using Solidariza.Models.Enum;

namespace Solidariza.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrganizationInfoController : ControllerBase
    {
        private readonly ConnectionDB _dbContext;
        private readonly ValidateOrganizationService _validateOrganizationService;

        public OrganizationInfoController(ConnectionDB dbContext, ValidateOrganizationService validateOrganizationService)
        {
            _dbContext = dbContext;
            _validateOrganizationService = validateOrganizationService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrganizationInfo>> GetOrganizationInfoById(int id)
        {
            OrganizationInfoService organizationInfoService = new OrganizationInfoService(_dbContext);
            OrganizationInfo? organizationInfo = await organizationInfoService.GetOrganizationInfoById(id);

            if (organizationInfo == null)
            {
                return NotFound();
            }

            return organizationInfo;
        }

        [HttpGet("Organization/{userId}")]
        public async Task<ActionResult<OrganizationInfo>> GetOrganizationInfoByUserId(int userId)
        {
            try
            {
                OrganizationInfoService organizationInfoService = new OrganizationInfoService(_dbContext);
                OrganizationInfo? organizationInfo = await organizationInfoService.GetOrganizationInfoByUserId(userId);

                if (organizationInfo == null)
                {
                    return NotFound();
                }

                return Ok(organizationInfo);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost("")]
        public async Task<ActionResult> CreateOrganizationInfo(NewOrganizationInfo newOrganizationInfo)
        {
            try
            {
                OrganizationInfoService organizationService = new OrganizationInfoService(_dbContext);
                OrganizationInfo organizationInfo = await organizationService.CreateOrganizationInfo(newOrganizationInfo);

                return Ok(organizationInfo);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrganizationInfo(int id, UpdateOrganizationInfo atualizarOrganizationInfo)
        {
            OrganizationInfoService organizationInfoService = new OrganizationInfoService(_dbContext);
            var existingOrganizationInfo = await _dbContext.Organization_Info.FindAsync(id);

            if (existingOrganizationInfo == null)
            {
                return NotFound();
            }

            OrganizationInfo organizationInfo = existingOrganizationInfo;
            organizationInfo.PixKey = atualizarOrganizationInfo.PixKey;
            organizationInfo.PixType = (PixType) Convert.ToInt32(atualizarOrganizationInfo.PixType);
            organizationInfo.BeneficiaryCity = atualizarOrganizationInfo.BeneficiaryCity;
            organizationInfo.BeneficiaryName = atualizarOrganizationInfo.BeneficiaryName;
            organizationInfo.ContactPhone = atualizarOrganizationInfo.ContactPhone;
            organizationInfo.ContactName = atualizarOrganizationInfo.ContactName;

            await organizationInfoService.AtualizarOrganizationInfo(existingOrganizationInfo, organizationInfo);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrganizationInfo(int id)
        {
            OrganizationInfoService organizationInfoService = new OrganizationInfoService(_dbContext);
            var organizationInfo = await _dbContext.Organization_Info.FindAsync(id);

            if (organizationInfo == null)
            {
                return NotFound();
            }

            await organizationInfoService.DeleteOrganizationInfo(organizationInfo);

            return NoContent();
        }

        [HttpPost("Validade/{organizationInfoId}/{cnpj}")]
        public async Task<ActionResult> ValidateOrganization(int organizationInfoId, string cnpj)
        {
            try
            {
                var existingOrganizationInfo = await _dbContext.Organization_Info.FindAsync(organizationInfoId);

                if (existingOrganizationInfo == null)
                {
                    return NotFound();
                }

                ConsultCnpjResponse organizationValid = await _validateOrganizationService.ConsultCNPJ(cnpj);

                OrganizationInfo organizationInfo = existingOrganizationInfo;
                organizationInfo.DisapprovalReason = organizationValid.DisapprovalReason;
                organizationInfo.IsOrganizationApproved = organizationValid.IsValid;

                OrganizationInfoService organizationInfoService = new OrganizationInfoService(_dbContext);
                OrganizationInfo organizationInfoResponse = await organizationInfoService.AtualizarOrganizationInfoValidate(existingOrganizationInfo, organizationInfo);

                return Ok(organizationInfoResponse);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
    }
}