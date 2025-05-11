using Solidariza.Services;
using Microsoft.AspNetCore.Mvc;
using Solidariza.Models;
using Microsoft.EntityFrameworkCore;

namespace Solidariza.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LinkController : ControllerBase
    {
        private readonly ConnectionDB _dbContext;

        public LinkController(ConnectionDB dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("Profile/{profileId}")]
        public async Task<ActionResult> GetLinksByProfileId(int profileId)
        {
            LinkService linkService = new LinkService(_dbContext);

            List<Link> link = await linkService.GetLinkByProfileId(profileId);

            return Ok(link);
        }

        [HttpPost("")]
        public async Task<ActionResult> CreateLink(NewLink newLink)
        {
            try
            {
                LinkService linkService = new LinkService(_dbContext);
                Link link = await linkService.CreateLink(newLink);

                return Ok(link);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLink(int id)
        {
            LinkService linkService = new LinkService(_dbContext);

            var link = await _dbContext.Link.FirstOrDefaultAsync(p => p.LinkId == id);

            if (link == null)
            {
                return NotFound();
            }

            await linkService.DeletarLink(link);

            return NoContent();
        }
    }
}