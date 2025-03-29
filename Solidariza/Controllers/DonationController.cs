using Solidariza;
using Solidariza.Services;
using Microsoft.AspNetCore.Mvc;
using Solidariza.Models;
using Solidariza.Models.Enum;

namespace Solidariza.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DonationController : ControllerBase
    {
        private readonly ConnectionDB _dbContext;

        public DonationController(ConnectionDB dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("QRCode/{campaignId}")]
        public async Task<ActionResult> GetDonationQRCode(int campaignId)
        {
            try
            {
                DonationService donationService = new DonationService(_dbContext);

                OrganizationInfo organizationInfo = await donationService.GetQRCodePixByCampaignId(campaignId);

                if (organizationInfo == null)
                {
                    return NotFound();
                }
                
                return Ok(organizationInfo);

            } catch(Exception e)
            {
                return Problem(e.Message);
            }

        }

    }

}