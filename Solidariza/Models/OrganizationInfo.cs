using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Solidariza.Models
{
    public class OrganizationInfo
    {
        public int OrganizationInfoId { get; set; }

        public int UserId { get; set; }

        public bool IsOrganizationApproved { get; set; } = false;

        public string DisapprovalReason { get; set; }

        public string PixKey { get; set; }

        public string ContactName { get; set; }

        public string ContactPhone { get; set; }

        public virtual User User { get; set; }
    }
}
