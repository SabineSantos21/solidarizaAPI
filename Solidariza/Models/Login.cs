﻿using System.ComponentModel.DataAnnotations;

namespace Solidariza.Models
{
    public class Login
    {
        public string Email { get; set; }

        public string Password { get; set; }
    }

    public class LoginResponse
    {
        public UserResponse User { get; set; }

        public string Token { get; set; }
    }
}
