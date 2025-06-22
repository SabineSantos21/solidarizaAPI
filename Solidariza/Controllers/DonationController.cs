using Microsoft.AspNetCore.Mvc;
using Solidariza.Models;
using Solidariza.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;

namespace Solidariza.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class DonationController : ControllerBase
    {
        private readonly IDonationService _donationService;

        public DonationController(IDonationService donationService)
        {
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