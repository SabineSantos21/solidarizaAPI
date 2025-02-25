using Solidariza.Models.Enum;

namespace Solidariza.Models
{
    public class Campaign
    {
        public int CampaignId { get; set; }

        public int UserId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public CampaignStatus Status { get; set; }

    }
}
