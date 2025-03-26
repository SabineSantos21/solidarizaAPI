using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Solidariza.Models.Enum;

namespace Solidariza.Models
{
    public class CampaignVolunteer
    {
        public int CampaignVolunteerId { get; set; }

        public int CampaignId { get; set; }

        public virtual Campaign Campaign { get; set; }

        public int UserId { get; set; }

        public virtual User User { get; set; }

        public CampaignVolunteerStatus IsApproved { get; set; }
    }

    public class NewCampaignVolunteer
    {
        public int CampaignId { get; set; }

        public int UserId { get; set; }
    }
    
    public class UpdateCampaignVolunteer
    {
        public int IsApproved { get; set; }

    }
}
