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

        public async Task<IEnumerable<Campaign>> GetCampaigns()
        {
            return await _dbContext.Campaign.ToListAsync();
        }

        public async Task<Campaign?> GetCampaignById(int id)
        {
            return await _dbContext.Campaign.FirstOrDefaultAsync(p => p.CampaignId == id);
        }

        public async Task<Campaign> CreateCampaign(NewCampaign newCampaign)
        {
            try
            {
                Campaign campaign = new Campaign()
                {
                    Title = newCampaign.Title,
                    Description = newCampaign.Description,
                    EndDate = newCampaign.EndDate,
                    StartDate = newCampaign.StartDate,
                    Status = (CampaignStatus)newCampaign.Status,
                };

                _dbContext.Campaign.Add(campaign);
                await _dbContext.SaveChangesAsync();

                return campaign;
            }
            catch (Exception ex)
            {
                throw ex;
            }
         
        }

        public async Task AtualizarCampaign(Campaign existingCampaign, Campaign campaign)
        {
            existingCampaign.Title = campaign.Title;
            existingCampaign.Description = campaign.Description;
            existingCampaign.EndDate = campaign.EndDate;
            existingCampaign.StartDate = campaign.StartDate;
            existingCampaign.Status = (CampaignStatus)campaign.Status;

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
