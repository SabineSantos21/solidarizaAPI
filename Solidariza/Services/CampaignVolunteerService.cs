using Solidariza.Models;
using Microsoft.EntityFrameworkCore;
using Solidariza.Models.Enum;

namespace Solidariza.Services
{
    public class CampaignVolunteerService
    {
        private readonly ConnectionDB _dbContext;

        public CampaignVolunteerService(ConnectionDB dbContext) 
        {
            _dbContext = dbContext;
        }

        public async Task<CampaignVolunteer?> GetCampaignVolunteerById(int id)
        {
            return await _dbContext.Campaign_Volunteers.Include(c => c.Campaign).Include(c => c.User).FirstOrDefaultAsync(p => p.CampaignVolunteerId == id);
        }

        public async Task<List<CampaignVolunteer>> GetCampaignVolunteersByCampaignId(int id)
        {
            return await _dbContext.Campaign_Volunteers.Include(c => c.Campaign).Include(c => c.User).Where(p => p.CampaignId == id).ToListAsync();
        }

        public async Task<CampaignVolunteer> CreateCampaignVolunteer(NewCampaignVolunteer newCampaignVolunteer)
        {
            try
            {
                CampaignVolunteer campaignVolunteer = new CampaignVolunteer()
                {
                    CampaignId = newCampaignVolunteer.CampaignId,
                    UserId = newCampaignVolunteer.UserId,
                    IsApproved = false
                };

                _dbContext.Campaign_Volunteers.Add(campaignVolunteer);
                await _dbContext.SaveChangesAsync();

                return campaignVolunteer;
            }
            catch (Exception ex)
            {
                throw ex;
            }
         
        }

        public async Task AtualizarCampaignVolunteer(CampaignVolunteer existingCampaignVolunteer, CampaignVolunteer campaignVolunteer)
        {
            try
            {
                existingCampaignVolunteer.IsApproved = true;
                
                _dbContext.Campaign_Volunteers.Update(existingCampaignVolunteer);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task DeletarCampaignVolunteer(CampaignVolunteer campaignVolunteer)
        {
            _dbContext.Campaign_Volunteers.Remove(campaignVolunteer);
            await _dbContext.SaveChangesAsync();
        }
    }
}
