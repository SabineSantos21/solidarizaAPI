using Solidariza.Models.Enum;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Solidariza.Models
{
    public class User
    {
        public int UserId { get; set; }

        public string? Name { get; set; }

        public UserType Type { get; set; }

        public DocumentType? DocumentType { get; set; }

        public string? DocumentNumber { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        [JsonIgnore]
        public string? Password { get; set; }

        public DateTime CreationDate { get; set; }

        public bool IsActive { get; set; } = true;

    }

    public class NewUser
    {
        public string? Name { get; set; }

        public int Type { get; set; }

        public int? DocumentType { get; set; }

        public string? DocumentNumber { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string? Password { get; set; }

        public string? ContactName { get; set; }

        public string? ContactPhone { get; set; }
    }

    public class UserResponse
    {
        public int UserId { get; set; }

        public string? Name { get; set; }

        public UserType Type { get; set; }

        public DocumentType? DocumentType { get; set; }

        public string? DocumentNumber { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
