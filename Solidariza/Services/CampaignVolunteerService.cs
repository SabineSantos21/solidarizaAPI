using Solidariza.Models;
using Microsoft.EntityFrameworkCore;
using Solidariza.Models.Enum;
using Solidariza.Interfaces.Services;

namespace Solidariza.Services
{
    public class CampaignVolunteerService: ICampaignVolunteerService
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
        
        public async Task<List<CampaignVolunteer>> GetCampaignVolunteersByUserId(int id)
        {
            return await _dbContext.Campaign_Volunteers.Include(c => c.Campaign).Include(c => c.User).Where(p => p.UserId == id).ToListAsync();
        }

        public async Task<List<CampaignVolunteer>> GetCampaignVolunteersByUserIdAndAproved(int id)
        {
            return await _dbContext.Campaign_Volunteers
                .Where(p =>
                    p.UserId == id &&
                    p.IsApproved == CampaignVolunteerStatus.APROVED &&
                    p.Campaign != null &&
                    p.User != null &&
                    p.Campaign.User != null
                )
                .Include(c => c.Campaign)
                .Include(c => c.User)
                .ToListAsync();
        }

        public async Task<CampaignVolunteer> CreateCampaignVolunteer(NewCampaignVolunteer newCampaignVolunteer)
        {

            CampaignVolunteer campaignVolunteer = new CampaignVolunteer()
            {
                CampaignId = newCampaignVolunteer.CampaignId,
                UserId = newCampaignVolunteer.UserId,
                IsApproved = CampaignVolunteerStatus.PENDING
            };

            _dbContext.Campaign_Volunteers.Add(campaignVolunteer);
            await _dbContext.SaveChangesAsync();

            return campaignVolunteer;
         
        }

        public async Task AtualizarCampaignVolunteer(CampaignVolunteer existingCampaignVolunteer, CampaignVolunteer campaignVolunteer)
        {

            existingCampaignVolunteer.IsApproved = campaignVolunteer.IsApproved;
                
            _dbContext.Campaign_Volunteers.Update(existingCampaignVolunteer);
            await _dbContext.SaveChangesAsync();

        }

        public async Task DeletarCampaignVolunteer(CampaignVolunteer campaignVolunteer)
        {
            _dbContext.Campaign_Volunteers.Remove(campaignVolunteer);
            await _dbContext.SaveChangesAsync();
        }
    }
}
