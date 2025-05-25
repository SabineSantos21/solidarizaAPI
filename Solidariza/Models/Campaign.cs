using Solidariza.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace Solidariza.Models
{
    public class Campaign
    {
        public int CampaignId { get; set; }

        public int UserId { get; set; }

        public virtual User? User { get; set; }

        public CampaignType Type { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public CampaignStatus Status { get; set; }

        public string? Address { get; set; }

        public string? City { get; set; }

        public string? State { get; set; }

    }

    public class NewCampaign
    {
        [Required]
        public int UserId { get; set; }

        public string? Title { get; set; }

        public int? Type { get; set; }

        public string? Description { get; set; }

        public string? StartDate { get; set; }

        public string? EndDate { get; set; }

        public int? Status { get; set; }

        public string? Address { get; set; }

        public string? City { get; set; }

        public string? State { get; set; }

    }
    
    public class UpdateCampaign
    {
        public int? UserId { get; set; }

        public string? Title { get; set; }

        public int? Type { get; set; }

        public string? Description { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int? Status { get; set; }

        public string? Address { get; set; }

        public string? City { get; set; }

        public string? State { get; set; }

    }
}
