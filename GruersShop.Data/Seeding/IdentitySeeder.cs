using GruersShop.Data.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace GruersShop.Data.Seeding;

    public static class IdentitySeeder
    {
        private const string DefaultPassword = "1234567890";

        // 1️⃣ Seed Roles
        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "Admin", "Manager", "User" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // 2️⃣ Seed Admin
        public static async Task SeedAdminAsync(UserManager<AppUser> userManager)
        {
            const string adminEmail = "admin@gruershop.com";
            const string adminAlternateEmail = "admin.alt@gruershop.com";
            var admin = await userManager.FindByEmailAsync(adminEmail);

            if (admin == null)
            {
                admin = new AppUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    AlternateEmail = adminAlternateEmail,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(admin, DefaultPassword);
            }

            if (!await userManager.IsInRoleAsync(admin, "Admin"))
                await userManager.AddToRoleAsync(admin, "Admin");
        }

        // 3️⃣ Seed Manager
        public static async Task SeedManagerAsync(UserManager<AppUser> userManager)
        {
            const string managerEmail = "manager@gruershop.com";
        const string managerAlternateEmail = "manager.alt@gruershop.com";
            var manager = await userManager.FindByEmailAsync(managerEmail);

            if (manager == null)
            {
                manager = new AppUser
                {
                    UserName = managerEmail,
                    Email = managerEmail,
                    AlternateEmail = managerAlternateEmail,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(manager, DefaultPassword);
            }

            if (!await userManager.IsInRoleAsync(manager, "Manager"))
                await userManager.AddToRoleAsync(manager, "Manager");
        }
    }