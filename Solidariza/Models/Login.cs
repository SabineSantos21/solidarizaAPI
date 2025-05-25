using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Solidariza.Models
{
    public class Login
    {
        [JsonRequired]
        [Required]
        public string? Email { get; set; }

        [JsonRequired]
        [Required]
        public string? Password { get; set; }
    }

    public class LoginResponse
    {
        public required UserResponse User { get; set; }

        public string? Token { get; set; }
    }
}
