using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Solidariza.Models
{
    public class User
    {
        public int UserId { get; set; }

        public string Name { get; set; }

        public int Type { get; set; }

        public int DocumentType { get; set; }

        public string DocumentNumber { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Password { get; set; }

        public DateTime CreationDate { get; set; }

        public bool IsActive { get; set; } = true;

    }

    public class NewUserOrganization
    {
        public string? Name { get; set; }

        public int Type { get; set; }

        public int DocumentType { get; set; }

        public string DocumentNumber { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Password { get; set; }
    }

    public class NewUserVolunteers
    {
        public string? Name { get; set; }

        public int Type { get; set; }

        public int DocumentType { get; set; }

        public string DocumentNumber { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Password { get; set; }
    }

    public class NewUserDonor
    {
        public string? Name { get; set; }

        public int Type { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Password { get; set; }
    }
}
