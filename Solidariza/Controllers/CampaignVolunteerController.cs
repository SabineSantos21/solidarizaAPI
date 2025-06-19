using Solidariza;
using Solidariza.Services;
using Microsoft.AspNetCore.Mvc;
using Solidariza.Models;
using Solidariza.Models.Enum;
using Solidariza.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;

namespace Solidariza.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class CampaignVolunteerController : ControllerBase
    {
        private readonly ConnectionDB _dbContext;
        private readonly ICampaignVolunteerService _campaignVolunteerService;

        public CampaignVolunteerController(ConnectionDB dbContext, ICampaignVolunteerService campaignVolunteerService)
        { 
            _dbContext = dbContext;
            _campaignVolunteerService = campaignVolunteerService;
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<CampaignVolunteer>> GetCampaignVolunteerById(int id)
        {
           
            CampaignVolunteer? campaignVolunteer = await _campaignVolunteerService.GetCampaignVolunteerById(id);

            if (campaignVolunteer == null)
            {
                return NotFound();
            }

            return Ok(campaignVolunteer);
        }

        [HttpGet("Campaign/{campaignId}")]
        public async Task<ActionResult<List<CampaignVolunteer>>> GetCampaignVolunteerByCampaignId(int campaignId)
        {
            List<CampaignVolunteer> campaignVolunteer = await _campaignVolunteerService.GetCampaignVolunteersByCampaignId(campaignId);

            if (campaignVolunteer == null)
            {
                return NotFound();
            }

            return Ok(campaignVolunteer);
        }
        
        [HttpGet("User/{userId}")]
        public async Task<ActionResult<List<CampaignVolunteer>>> GetCampaignVolunteerByUserId(int userId)
        {
            List<CampaignVolunteer> campaignVolunteer = await _campaignVolunteerService.GetCampaignVolunteersByUserId(userId);

            if (campaignVolunteer == null)
            {
                return NotFound();
            }

            return Ok(campaignVolunteer);
        }
        
        [HttpGet("User/{userId}/Aproved")]
        public async Task<ActionResult<List<CampaignVolunteer>>> GetCampaignVolunteerByUserIdAndAproved(int userId)
        {
            List<CampaignVolunteer> campaignVolunteer = await _campaignVolunteerService.GetCampaignVolunteersByUserIdAndAproved(userId);

            if (campaignVolunteer == null)
            {
                return NotFound();
            }

            return Ok(campaignVolunteer);
        }

        [HttpPost("")]
        public async Task<ActionResult> CreateCampaignVolunteer(NewCampaignVolunteer newCampaignVolunteer)
        {
            try
            {
                var existingCampaignVolunteer = _dbContext.Campaign_Volunteers
                 .Where(c => c.CampaignId == newCampaignVolunteer.CampaignId && c.UserId == newCampaignVolunteer.UserId)
                 .ToList();

                if (existingCampaignVolunteer.Count > 0)
                {
                    var campaignVolunteerCreated = existingCampaignVolunteer[0];
                    return Ok(campaignVolunteerCreated);
                }

                CampaignVolunteer campaignVolunteer = await _campaignVolunteerService.CreateCampaignVolunteer(newCampaignVolunteer);

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
            var existingCampaignVolunteer = await _dbContext.Campaign_Volunteers.FindAsync(id);

            if (existingCampaignVolunteer == null)
            {
                return NotFound();
            }

            CampaignVolunteer campaign = new CampaignVolunteer()
            {
                IsApproved = (CampaignVolunteerStatus) Convert.ToInt32(atualizarCampaignVolunteer.IsApproved)
            };

            await _campaignVolunteerService.AtualizarCampaignVolunteer(existingCampaignVolunteer, campaign);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCampaignVolunteer(int id)
        {
            var campaignVolunteer = await _dbContext.Campaign_Volunteers.FindAsync(id);

            if (campaignVolunteer == null)
            {
                return NotFound();
            }

            await _campaignVolunteerService.DeletarCampaignVolunteer(campaignVolunteer);

            return NoContent();
        }
    }
}