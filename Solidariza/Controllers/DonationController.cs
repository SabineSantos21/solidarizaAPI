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

        [HttpPost("QRCode")]
        public async Task<ActionResult> GetDonationQRCode(DonationQRCodeRequest donationQRCodeRequest)
        {
            try
            {
                DonationService donationService = new DonationService(_dbContext);

                string donationQRCode = await donationService.GetDonationQRCode(donationQRCodeRequest);

                if (donationQRCode == null)
                {
                    return NotFound();
                }
                
                return Ok(donationQRCode);

            } catch(Exception e)
            {
                return Problem(e.Message);
            }

        }

    }

}