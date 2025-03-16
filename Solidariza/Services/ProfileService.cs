using Solidariza.Models;
using Microsoft.EntityFrameworkCore;
using Solidariza.Models.Enum;

namespace Solidariza.Services
{
    public class ProfileService
    {
        private readonly ConnectionDB _dbContext;

        public ProfileService(ConnectionDB dbContext) 
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Profile>> GetProfiles()
        {
            return await _dbContext.Profile.ToListAsync();
        }

        public async Task<Profile?> GetProfileById(int id)
        {
            return await _dbContext.Profile.FirstOrDefaultAsync(p => p.ProfileId == id);
        }

        public async Task<Profile?> GetProfileByUserId(int userId)
        {
            return await _dbContext.Profile.FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task<Profile> CreateProfile(NewProfile newProfile)
        {
            try
            {
                Profile profile = new Profile()
                {
                    Name = newProfile.Name,
                    Description = newProfile.Description,
                    Address = newProfile.Address,
                    UserId = newProfile.UserId,
                };

                _dbContext.Profile.Add(profile);
                await _dbContext.SaveChangesAsync();

                return profile;
            }
            catch (Exception ex)
            {
                throw ex;
            }
         
        }

        public async Task AtualizarProfile(Profile existingProfile, Profile profile)
        {
            existingProfile.Name = profile.Name;
            existingProfile.Description = profile.Description;
            existingProfile.Address = profile.Address;
            existingProfile.UserId = profile.UserId;

            _dbContext.Profile.Update(existingProfile);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeletarProfile(Profile profile)
        {
            _dbContext.Profile.Remove(profile);
            await _dbContext.SaveChangesAsync();
        }
    }
}
