using Solidariza;
using Solidariza.Services;
using Microsoft.AspNetCore.Mvc;
using Solidariza.Models;
using Solidariza.Models.Enum;

namespace Solidariza.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CampaignVolunteerController : ControllerBase
    {
        private readonly ConnectionDB _dbContext;

        public CampaignVolunteerController(ConnectionDB dbContext)
        {
            _dbContext = dbContext;
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<CampaignVolunteer>> GetCampaignVolunteerById(int id)
        {
            CampaignVolunteerService campaignVolunteerService = new CampaignVolunteerService(_dbContext);

            CampaignVolunteer? campaignVolunteer = await campaignVolunteerService.GetCampaignVolunteerById(id);

            if (campaignVolunteer == null)
            {
                return NotFound();
            }

            return campaignVolunteer;
        }

        [HttpGet("Campaign/{campaignId}")]
        public async Task<ActionResult<List<CampaignVolunteer>>> GetCampaignVolunteerByCampaignId(int campaignId)
        {
            CampaignVolunteerService campaignVolunteerService = new CampaignVolunteerService(_dbContext);

            List<CampaignVolunteer> campaignVolunteer = await campaignVolunteerService.GetCampaignVolunteersByCampaignId(campaignId);

            if (campaignVolunteer == null)
            {
                return NotFound();
            }

            return campaignVolunteer;
        }
        
        [HttpGet("User/{userId}")]
        public async Task<ActionResult<List<CampaignVolunteer>>> GetCampaignVolunteerByUserId(int userId)
        {
            CampaignVolunteerService campaignVolunteerService = new CampaignVolunteerService(_dbContext);

            List<CampaignVolunteer> campaignVolunteer = await campaignVolunteerService.GetCampaignVolunteersByUserId(userId);

            if (campaignVolunteer == null)
            {
                return NotFound();
            }

            return campaignVolunteer;
        }
        
        [HttpGet("User/{userId}/Aproved")]
        public async Task<ActionResult<List<CampaignVolunteer>>> GetCampaignVolunteerByUserIdAndAproved(int userId)
        {
            CampaignVolunteerService campaignVolunteerService = new CampaignVolunteerService(_dbContext);

            List<CampaignVolunteer> campaignVolunteer = await campaignVolunteerService.GetCampaignVolunteersByUserIdAndAproved(userId);

            if (campaignVolunteer == null)
            {
                return NotFound();
            }

            return campaignVolunteer;
        }

        [HttpPost("")]
        public async Task<ActionResult> CreateCampaignVolunteer(NewCampaignVolunteer newCampaignVolunteer)
        {
            try
            {
                CampaignVolunteerService campaignVolunteerService = new CampaignVolunteerService(_dbContext);

                var existingCampaignVolunteer = _dbContext.Campaign_Volunteers
                 .Where(c => c.CampaignId == newCampaignVolunteer.CampaignId && c.UserId == newCampaignVolunteer.UserId)
                 .ToList();

                if (existingCampaignVolunteer.Count > 0)
                {
                    var campaignVolunteerCreated = existingCampaignVolunteer[0];
                    return Ok(campaignVolunteerCreated);
                }

                CampaignVolunteer campaignVolunteer = await campaignVolunteerService.CreateCampaignVolunteer(newCampaignVolunteer);

                return Ok(campaignVolunteer);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCampaignVolunteer(int id, UpdateCampaignVolunteer atualizarCampaignVolunteer)
        {
            CampaignVolunteerService campaignVolunteerService = new CampaignVolunteerService(_dbContext);

            var existingCampaignVolunteer = await _dbContext.Campaign_Volunteers.FindAsync(id);

            if (existingCampaignVolunteer == null)
            {
                return NotFound();
            }

            CampaignVolunteer campaign = new CampaignVolunteer()
            {
                IsApproved = (CampaignVolunteerStatus) Convert.ToInt32(atualizarCampaignVolunteer.IsApproved)
            };

            await campaignVolunteerService.AtualizarCampaignVolunteer(existingCampaignVolunteer, campaign);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCampaignVolunteer(int id)
        {
            CampaignVolunteerService campaignVolunteerService = new CampaignVolunteerService(_dbContext);

            var campaignVolunteer = await _dbContext.Campaign_Volunteers.FindAsync(id);

            if (campaignVolunteer == null)
            {
                return NotFound();
            }

            await campaignVolunteerService.DeletarCampaignVolunteer(campaignVolunteer);

            return NoContent();
        }
    }
}