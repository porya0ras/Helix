﻿using Microsoft.AspNetCore.Identity;

namespace helix.Models
{
    public class User : IdentityUser
    {
        public string Type { get; set; } = "General";
        public string Surname { get; set; }
        public string LastName { get; set; }
        public string? Institution { get; set; }
    }
}