using GruersShop.Data.Models.Base;
using Microsoft.AspNetCore.Identity;

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

            var result = await userManager.CreateAsync(admin, DefaultPassword);
            if (!result.Succeeded)
            {
                throw new Exception($"Admin creation failed: {string.Join(", ", result.Errors)}");
            }

            // Ре-зареждане на потребителя, за да сме сигурни, че Id е генериран
            admin = await userManager.FindByEmailAsync(adminEmail) ?? throw new Exception("Admin not found after creation");
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

            var result = await userManager.CreateAsync(manager, DefaultPassword);
            if (!result.Succeeded)
            {
                throw new Exception($"Manager creation failed: {string.Join(", ", result.Errors)}");
            }

            // re-load the user to ensure the Id is generated
            manager = await userManager.FindByEmailAsync(managerEmail) ?? throw new Exception("Manager not found after creation");
        }

        if (!await userManager.IsInRoleAsync(manager, "Manager"))
            await userManager.AddToRoleAsync(manager, "Manager");
    }
}