using GruersShop.Data.Models.Base;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace GruersShop.Data.Seeding;

public static class IdentitySeeder
{
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
    public static async Task SeedAdminAsync(
        UserManager<AppUser> userManager,
        IConfiguration configuration)
    {
        var adminEmail = configuration["SeedData:AdminEmail"]
            ?? throw new InvalidOperationException("SeedData:AdminEmail is missing");
        var adminPassword = configuration["SeedData:AdminPassword"]
            ?? throw new InvalidOperationException("SeedData:AdminPassword is missing");

        var admin = await userManager.FindByEmailAsync(adminEmail);

        if (admin == null)
        {
            admin = new AppUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                AlternateEmail = "admin.alt@gruershop.com",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(admin, adminPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Admin creation failed: {errors}");
            }

            admin = await userManager.FindByEmailAsync(adminEmail)
                ?? throw new Exception("Admin not found after creation");
        }

        if (!await userManager.IsInRoleAsync(admin, "Admin"))
            await userManager.AddToRoleAsync(admin, "Admin");
    }

    // 3️⃣ Seed Manager
    public static async Task SeedManagerAsync(
        UserManager<AppUser> userManager,
        IConfiguration configuration)
    {
        var managerEmail = configuration["SeedData:ManagerEmail"]
            ?? throw new InvalidOperationException("SeedData:ManagerEmail is missing");
        var managerPassword = configuration["SeedData:ManagerPassword"]
            ?? throw new InvalidOperationException("SeedData:ManagerPassword is missing");

        var manager = await userManager.FindByEmailAsync(managerEmail);

        if (manager == null)
        {
            manager = new AppUser
            {
                UserName = managerEmail,
                Email = managerEmail,
                AlternateEmail = "manager.alt@gruershop.com",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(manager, managerPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Manager creation failed: {errors}");
            }

            manager = await userManager.FindByEmailAsync(managerEmail)
                ?? throw new Exception("Manager not found after creation");
        }

        if (!await userManager.IsInRoleAsync(manager, "Manager"))
            await userManager.AddToRoleAsync(manager, "Manager");
    }
}