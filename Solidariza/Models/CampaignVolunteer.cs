using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Solidariza.Models
{
    public class CampaignVolunteer
    {
        public int CampaignVolunteerId { get; set; }

        public int CampaignId { get; set; }

        public virtual Campaign Campaign { get; set; }

        public int UserId { get; set; }

        public virtual User User { get; set; }

        public bool IsApproved { get; set; } = false;
    }

    public class NewCampaignVolunteer
    {
        public int CampaignId { get; set; }

        public int UserId { get; set; }
    }
    
    public class UpdateCampaignVolunteer
    {
        public bool IsApproved { get; set; } = false;

    }
}
