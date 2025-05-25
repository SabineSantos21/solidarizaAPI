using Solidariza.Models;

namespace Solidariza.Interfaces.Services
{
    public interface ILinkService
    {
        Task<List<Link>> GetLinkByProfileId(int profileId);
        Task<Link> CreateLink(NewLink newLink);
        Task DeletarLink(Link link);
    }
}
