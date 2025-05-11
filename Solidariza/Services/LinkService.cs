using Solidariza.Models;
using Microsoft.EntityFrameworkCore;
using Solidariza.Models.Enum;

namespace Solidariza.Services
{
    public class LinkService
    {
        private readonly ConnectionDB _dbContext;

        public LinkService(ConnectionDB dbContext) 
        {
            _dbContext = dbContext;
        }

        public Task<List<Link>> GetLinkByProfileId(int profileId)
        {
            return _dbContext.Link.Where(p => p.ProfileId == profileId).ToListAsync();
        }

        public async Task<Link> CreateLink(NewLink newLink)
        {

            Link link = new Link()
            {
                Url = newLink.Url,
                Type = newLink.Type,
                ProfileId = newLink.ProfileId,
            };

            _dbContext.Link.Add(link);
            await _dbContext.SaveChangesAsync();

            return link;
         
        }

        public async Task DeletarLink(Link link)
        {
            _dbContext.Link.Remove(link);
            await _dbContext.SaveChangesAsync();
        }
    }
}
