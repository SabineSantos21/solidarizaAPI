using Solidariza.Models;

namespace Solidariza.Interfaces.Services
{
    public interface ICampaignVolunteerService
    {
        Task<CampaignVolunteer?> GetCampaignVolunteerById(int id);
        
        Task<List<CampaignVolunteer>> GetCampaignVolunteersByCampaignId(int id);
        
        Task<List<CampaignVolunteer>> GetCampaignVolunteersByUserId(int id);
        
        Task<List<CampaignVolunteer>> GetCampaignVolunteersByUserIdAndAproved(int id);
        
        Task<CampaignVolunteer> CreateCampaignVolunteer(NewCampaignVolunteer newCampaignVolunteer);

        Task AtualizarCampaignVolunteer(CampaignVolunteer existingCampaignVolunteer, CampaignVolunteer campaignVolunteer);
        
        Task DeletarCampaignVolunteer(CampaignVolunteer campaignVolunteer);
    }
}
