﻿using Microsoft.AspNetCore.Identity;

namespace EventManager.Models.ViewModels
{
    public class UserViewModel
    {
        public IEnumerable<AppUser> Users { get; set; }

        public IEnumerable<IdentityRole> Roles { get; set; }
    }
}
