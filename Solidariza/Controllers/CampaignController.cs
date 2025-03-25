using Solidariza;
using Solidariza.Services;
using Microsoft.AspNetCore.Mvc;
using Solidariza.Models;
using Solidariza.Models.Enum;

namespace Solidariza.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CampaignController : ControllerBase
    {
        private readonly ConnectionDB _dbContext;

        public CampaignController(ConnectionDB dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("")]
        public async Task<ActionResult<List<Campaign>>> GetCampaigns()
        {
            CampaignService campaignService = new CampaignService(_dbContext);

            List<Campaign> campaign = await campaignService.GetCampaigns();

            if (campaign == null)
            {
                return NotFound();
            }

            return campaign;
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<Campaign>> GetCampaignById(int id)
        {
            CampaignService campaignService = new CampaignService(_dbContext);

            Campaign? campaign = await campaignService.GetCampaignById(id);

            if (campaign == null)
            {
                return NotFound();
            }

            return campaign;
        }

        [HttpGet("User/{userId}")]
        public async Task<ActionResult<List<Campaign>>> GetCampaignByUserId(int userId)
        {
            CampaignService campaignService = new CampaignService(_dbContext);

            List<Campaign> campaign = await campaignService.GetCampaignByUserId(userId);

            if (campaign == null)
            {
                return NotFound();
            }

            return campaign;
        }

        [HttpPost("")]
        public async Task<ActionResult> CreateCampaign(NewCampaign newCampaign)
        {
            try
            {
                CampaignService campaignService = new CampaignService(_dbContext);
                Campaign campaign = await campaignService.CreateCampaign(newCampaign);

                return Ok(campaign);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCampaign(int id, UpdateCampaign atualizarCampaign)
        {
            CampaignService campaignService = new CampaignService(_dbContext);

            var existingCampaign = await _dbContext.Campaign.FindAsync(id);

            if (existingCampaign == null)
            {
                return NotFound();
            }

            Campaign campaign = new Campaign();
            campaign.Title = atualizarCampaign.Title;
            campaign.Description = atualizarCampaign.Description;
            campaign.StartDate = Convert.ToDateTime(atualizarCampaign.StartDate);
            campaign.EndDate = Convert.ToDateTime(atualizarCampaign.EndDate);
            campaign.Status = (CampaignStatus) atualizarCampaign.Status;

            await campaignService.AtualizarCampaign(existingCampaign, campaign);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCampaign(int id)
        {
            CampaignService campaignService = new CampaignService(_dbContext);

            var campaign = await _dbContext.Campaign.FindAsync(id);

            if (campaign == null)
            {
                return NotFound();
            }

            await campaignService.DeletarCampaign(campaign);

            return NoContent();
        }
    }
}