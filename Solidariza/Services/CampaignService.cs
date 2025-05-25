using Solidariza.Models;
using Microsoft.EntityFrameworkCore;
using Solidariza.Models.Enum;

namespace Solidariza.Services
{
    public class CampaignService
    {
        private readonly ConnectionDB _dbContext;

        public CampaignService(ConnectionDB dbContext) 
        {
            _dbContext = dbContext;
        }

        public async Task<List<Campaign>> GetCampaigns()
        {
            return await _dbContext.Campaign
                .Where(c => c.Status == CampaignStatus.Active)
                .Join(_dbContext.Organization_Info,
                      campaign => campaign.UserId,
                      org => org.UserId,
                      (campaign, org) => new { campaign, org })
                .Where(x => x.org.IsOrganizationApproved) // Filtra apenas as organizações aprovadas
                .Select(x => x.campaign) // Retorna apenas as campanhas
                .Include(c => c.User) // Inclui os usuários das campanhas
                .ToListAsync();
        }

        public async Task<Campaign?> GetCampaignById(int id)
        {
            return await _dbContext.Campaign.Include(p => p.User).FirstOrDefaultAsync(p => p.CampaignId == id);
        }

        public async Task<List<Campaign>> GetCampaignByUserId(int id)
        {
            return await _dbContext.Campaign.Where(p => p.UserId == id).Include(p => p.User).ToListAsync();
        }

        public async Task<Campaign> CreateCampaign(NewCampaign newCampaign)
        {

            Campaign campaign = new Campaign()
            {
                Title = newCampaign.Title,
                Description = newCampaign.Description,
                EndDate = Convert.ToDateTime(newCampaign.EndDate),
                StartDate = Convert.ToDateTime(newCampaign.StartDate),
                Status = (CampaignStatus) Convert.ToInt32(newCampaign.Status),
                UserId = newCampaign.UserId,
                Type = (CampaignType)Convert.ToInt32(newCampaign.Type),
                Address = newCampaign.Address,
                State = newCampaign.State,
                City = newCampaign.City,
            };

            _dbContext.Campaign.Add(campaign);
            await _dbContext.SaveChangesAsync();

            return campaign;
         
        }

        public async Task AtualizarCampaign(Campaign existingCampaign, Campaign campaign)
        {

            existingCampaign.Title = campaign.Title;
            existingCampaign.Description = campaign.Description;
            existingCampaign.EndDate = campaign.EndDate;
            existingCampaign.StartDate = campaign.StartDate;
            existingCampaign.Status = campaign.Status;
            existingCampaign.Type = campaign.Type;
            existingCampaign.State = campaign.State;
            existingCampaign.City = campaign.City;
            existingCampaign.Address = campaign.Address;

            _dbContext.Campaign.Update(existingCampaign);
            await _dbContext.SaveChangesAsync();

        }

        public async Task DeletarCampaign(Campaign campaign)
        {
            _dbContext.Campaign.Remove(campaign);
            await _dbContext.SaveChangesAsync();
        }
    }
}
