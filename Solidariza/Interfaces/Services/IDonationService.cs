using Solidariza.Models;

namespace Solidariza.Interfaces.Services
{
    public interface IDonationService
    {
        Task<OrganizationInfo> GetQRCodePixByCampaignId(int campaignId);
        Task<string> GetDonationQRCode(DonationQRCodeRequest donationQRCodeRequest);
    }
}
