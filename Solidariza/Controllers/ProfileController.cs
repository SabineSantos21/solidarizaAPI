using Solidariza;
using Solidariza.Services;
using Microsoft.AspNetCore.Mvc;
using Solidariza.Models;
using Solidariza.Models.Enum;

namespace Solidariza.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly ConnectionDB _dbContext;

        public ProfileController(ConnectionDB dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Profile>> GetProfileById(int id)
        {
            ProfileService profileService = new ProfileService(_dbContext);

            Profile? profile = await profileService.GetProfileById(id);

            if (profile == null)
            {
                return NotFound();
            }

            return profile;
        }

        [HttpGet("User/{userId}")]
        public async Task<ActionResult<Profile>> GetProfileByUserId(int userId)
        {
            try
            {
                ProfileService profileService = new ProfileService(_dbContext);

                Profile? profile = await profileService.GetProfileByUserId(userId);

                if (profile == null)
                {
                    return NotFound();
                }

                return profile;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }

        [HttpPost("")]
        public async Task<ActionResult> CreateProfile(NewProfile newProfile)
        {
            try
            {
                ProfileService profileService = new ProfileService(_dbContext);
                Profile profile = await profileService.CreateProfile(newProfile);

                return Ok(profile);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProfile(int id, UpdateProfile atualizarProfile)
        {
            ProfileService profileService = new ProfileService(_dbContext);

            var existingProfile = await _dbContext.Profile.FindAsync(id);

            if (existingProfile == null)
            {
                return NotFound();
            }

            Profile profile = existingProfile;
            profile.Name = atualizarProfile.Name;
            profile.Phone = atualizarProfile.Phone;
            profile.Description = atualizarProfile.Description;
            profile.Address = atualizarProfile.Address;
            profile.City = atualizarProfile.City;
            profile.State = atualizarProfile.State;
            profile.Zip = atualizarProfile.Zip;

            await profileService.AtualizarProfile(existingProfile, profile);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProfile(int id)
        {
            ProfileService profileService = new ProfileService(_dbContext);

            var profile = await _dbContext.Profile.FindAsync(id);

            if (profile == null)
            {
                return NotFound();
            }

            await profileService.DeletarProfile(profile);

            return NoContent();
        }
    }
}