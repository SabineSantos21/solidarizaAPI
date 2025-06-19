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
    public class CampaignController : ControllerBase
    {
        private readonly ConnectionDB _dbContext;
        private readonly ICampaignService _campaignService;

        public CampaignController(ConnectionDB dbContext, ICampaignService campaignService)
        {
            _dbContext = dbContext;
            _campaignService = campaignService;
        }

        [HttpGet("")]
        public async Task<ActionResult<List<Campaign>>> GetCampaigns()
        {

            List<Campaign> campaign = await _campaignService.GetCampaigns();

            if (campaign == null)
            {
                return NotFound();
            }

            return Ok(campaign);
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<Campaign>> GetCampaignById(int id)
        {

            Campaign? campaign = await _campaignService.GetCampaignById(id);

            if (campaign == null)
            {
                return NotFound();
            }

            return campaign;
        }

        [HttpGet("User/{userId}")]
        public async Task<ActionResult<List<Campaign>>> GetCampaignByUserId(int userId)
        {
            List<Campaign> campaign = await _campaignService.GetCampaignByUserId(userId);

            if (campaign == null)
            {
                return NotFound();
            }

            return Ok(campaign);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCampaign([FromBody] NewCampaign newCampaign)
        {
            try
            {
                var campaign = await _campaignService.CreateCampaign(newCampaign);
                return Ok(campaign);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCampaign(int id, UpdateCampaign atualizarCampaign)
        {
            var existingCampaign = await _dbContext.Campaign.FindAsync(id);

            if (existingCampaign == null)
            {
                return NotFound();
            }

            // Cria um objeto temporário só para passar os dados, sem trackear no DbContext
            var updatedCampaign = new Campaign
            {
                Title = atualizarCampaign.Title,
                Description = atualizarCampaign.Description,
                StartDate = Convert.ToDateTime(atualizarCampaign.StartDate),
                EndDate = Convert.ToDateTime(atualizarCampaign.EndDate),
                Status = (CampaignStatus)Convert.ToInt32(atualizarCampaign.Status),
                Type = (CampaignType)Convert.ToInt32(atualizarCampaign.Type),
                State = atualizarCampaign.State,
                City = atualizarCampaign.City,
                Address = atualizarCampaign.Address
            };

            await _campaignService.AtualizarCampaign(existingCampaign, updatedCampaign);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCampaign(int id)
        {
            var campaign = await _dbContext.Campaign.FindAsync(id);

            if (campaign == null)
            {
                return NotFound();
            }

            await _campaignService.DeletarCampaign(campaign);

            return NoContent();
        }
    }
}