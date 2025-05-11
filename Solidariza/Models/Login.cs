using System.ComponentModel.DataAnnotations;

namespace Solidariza.Models
{
    public class Login
    {
        [Required]
        public string? Email { get; set; }

        [Required]
        public string? Password { get; set; }
    }

    public class LoginResponse
    {
        public required UserResponse User { get; set; }

        public string? Token { get; set; }
    }
}
