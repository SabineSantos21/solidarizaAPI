using Solidariza.Models;

namespace Solidariza.Interfaces.Services
{
    public interface IProfileService
    {
        Task<IEnumerable<Profile>> GetProfiles();
        Task<Profile?> GetProfileById(int id);
        Task<Profile?> GetProfileByUserId(int userId);
        Task<List<Profile>> GetProfilesOrganizationGetProfilesOrganization();
        Task<Profile> CreateProfile(NewProfile newProfile);
        Task AtualizarProfile(Profile existingProfile, Profile profile);
        Task DeletarProfile(Profile profile);
    }
}
