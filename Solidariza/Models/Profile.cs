﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Solidariza.Models
{
    public class Profile
    {
        public int ProfileId { get; set; }

        public int UserId { get; set; }

        public virtual User? User { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public string? Address { get; set; }

        public string? City { get; set; }
        
        public string? State { get; set; }
        
        public string? Zip { get; set; }

        public string? Phone { get; set; }

    }
    
    public class NewProfile
    {
        [JsonRequired]
        [Required]
        public int UserId { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public string? Address { get; set; }

        public string? City { get; set; }

        public string? State { get; set; }

        public string? Zip { get; set; }

        public string? Phone { get; set; }
    }
    
    public class UpdateProfile
    {
        public string? Name { get; set; }

        public string? Description { get; set; }

        public string? Address { get; set; }

        public string? City { get; set; }

        public string? State { get; set; }

        public string? Zip { get; set; }

        public string? Phone { get; set; }
    }
}
