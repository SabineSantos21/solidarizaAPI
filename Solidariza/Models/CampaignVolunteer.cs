using Solidariza.Models.Enum;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Solidariza.Models
{
    public class CampaignVolunteer
    {
        public int CampaignVolunteerId { get; set; }

        public int CampaignId { get; set; }

        public virtual Campaign? Campaign { get; set; }

        public int UserId { get; set; }

        public virtual User? User { get; set; }

        public CampaignVolunteerStatus IsApproved { get; set; }
    }

    public class NewCampaignVolunteer
    {
        [JsonRequired]
        [Required]
        public int CampaignId { get; set; }

        [JsonRequired]
        [Required]
        public int UserId { get; set; }
    }
    
    public class UpdateCampaignVolunteer
    {
        public int? IsApproved { get; set; }

    }
}
