using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Solidariza.Models
{
    public class CampaignVolunteer
    {

        public int CampaignId { get; set; }

        public int UserId { get; set; }

        public bool IsApproved { get; set; } = false;
    }
}
