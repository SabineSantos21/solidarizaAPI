using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Solidariza.Models.Enum;

namespace Solidariza.Models
{
    public class OrganizationInfo
    {
        public int OrganizationInfoId { get; set; }

        public int UserId { get; set; }

        public bool IsOrganizationApproved { get; set; }

        public string? DisapprovalReason { get; set; }

        public PixType PixType { get; set; }

        public string PixKey { get; set; }

        public string BeneficiaryName { get; set; }

        public string BeneficiaryCity { get; set; }

        public string? PixValue { get; set; }

        public string ContactName { get; set; }

        public string ContactPhone { get; set; }

        public virtual User User { get; set; }
    }
    
    public class NewOrganizationInfo
    {
        public int UserId { get; set; }

        public int PixType { get; set; }

        public string PixKey { get; set; }

        public string BeneficiaryName { get; set; }

        public string BeneficiaryCity { get; set; }

        public string? PixValue { get; set; }

        public string ContactName { get; set; }

        public string ContactPhone { get; set; }
    }

    public class UpdateOrganizationInfo
    {
        public int PixType { get; set; }

        public string PixKey { get; set; }

        public string BeneficiaryName { get; set; }

        public string BeneficiaryCity { get; set; }

        public string? PixValue { get; set; }

        public string ContactName { get; set; }

        public string ContactPhone { get; set; }
    }
}
