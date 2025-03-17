﻿using EventManager.Models;
using Microsoft.AspNetCore.Identity;

namespace EventManager.Data
{
    public class ConfigureIdentity
    {
        private static RoleManager<IdentityRole> roleManager;
        private static UserManager<AppUser> userManager;

        public static async Task CreateAdminUserAsync(IServiceProvider provider)
        {
            roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
            userManager = provider.GetRequiredService<UserManager<AppUser>>();

            const string PLAYER = "Player";
            await CreateRole(PLAYER);
            const string ADMIN = "Admin";
            await CreateRole(ADMIN);
            const string PASSWORD = "Secret123!";
            await CreateUser("admin", "", PASSWORD, ADMIN);
            await CreateUser("Gabe", "Gucker", PASSWORD, PLAYER);
            await CreateUser("Sam", "Black", PASSWORD, PLAYER);
            await CreateUser("Reid", "Duke", PASSWORD, PLAYER);
            await CreateUser("Patrick", "Chapin", PASSWORD, PLAYER);
            await CreateUser("Paul", "Cheon", PASSWORD, PLAYER);
            await CreateUser("Brodie", "Spurlock", PASSWORD, PLAYER);
            await CreateUser("Zak", "Miller", PASSWORD, PLAYER);

        }

        private static async Task CreateRole(string roleName)
        {
            if (await roleManager.FindByNameAsync(roleName) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        private static async Task CreateUser(string firstName, string lastName, string password, string role)
        {
            AppUser user = new AppUser
            {
                UserName = firstName + lastName,
                Name = firstName + " " + lastName
            };
            var result = await userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, role);
            }
        }
    }
}
