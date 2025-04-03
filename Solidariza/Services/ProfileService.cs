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
            try
            {
                return await _dbContext.Profile.Where(p => p.UserId == userId).FirstOrDefaultAsync();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Profile>> GetProfilesOrganizationGetProfilesOrganization()
        {
            return await _dbContext.Profile.Include(c => c.User).Where(p => p.User.Type == UserType.Organization).ToListAsync();
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
                    Zip = newProfile.Zip,
                    City = newProfile.City,
                    State = newProfile.State,
                    Phone = newProfile.Phone,
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
            try
            {
                existingProfile.Name = profile.Name;
                existingProfile.Description = profile.Description;
                existingProfile.Address = profile.Address;
                existingProfile.UserId = profile.UserId;
                existingProfile.Zip = profile.Zip;
                existingProfile.City = profile.City;
                existingProfile.State = profile.State;
                existingProfile.Phone = profile.Phone;

                _dbContext.Profile.Update(existingProfile);
                await _dbContext.SaveChangesAsync();

                UserService userService = new UserService(_dbContext);
                User? existingUser = await userService.GetUserById(existingProfile.UserId);

                if(existingUser != null)
                {
                    existingUser.Name = profile.Name;

                    _dbContext.User.Update(existingUser);
                    await _dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task DeletarProfile(Profile profile)
        {
            _dbContext.Profile.Remove(profile);
            await _dbContext.SaveChangesAsync();
        }
    }
}
