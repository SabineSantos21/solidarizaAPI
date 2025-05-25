using Solidariza.Models;

namespace Solidariza.Interfaces.Services
{
    public interface ICampaignService
    {
        Task<List<Campaign>> GetCampaigns();

        Task<Campaign?> GetCampaignById(int id);

        Task<List<Campaign>> GetCampaignByUserId(int id);

        Task<Campaign> CreateCampaign(NewCampaign newCampaign);

        Task AtualizarCampaign(Campaign existingCampaign, Campaign campaign);

        Task DeletarCampaign(Campaign campaign);
    }
}
