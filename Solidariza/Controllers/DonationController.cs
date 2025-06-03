using Solidariza;
using Solidariza.Services;
using Microsoft.AspNetCore.Mvc;
using Solidariza.Models;
using Solidariza.Models.Enum;
using Solidariza.Interfaces.Services;

namespace Solidariza.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DonationController : ControllerBase
    {
        private readonly ConnectionDB _dbContext;
        private readonly IDonationService _donationService;

        public DonationController(ConnectionDB dbContext, IDonationService donationService)
        {
            _dbContext = dbContext;
            _donationService = donationService;
        }

        [HttpGet("QRCode/{campaignId}")]
        public async Task<ActionResult> GetDonationQRCode(int campaignId)
        {
            try
            {
                OrganizationInfo organizationInfo = await _donationService.GetQRCodePixByCampaignId(campaignId);

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