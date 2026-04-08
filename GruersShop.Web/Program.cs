

using GruersShop.Data;
using GruersShop.Data.Models;
using GruersShop.Data.Repositories.Interfaces.Account;
using GruersShop.Data.Repositories.Interfaces.CRUD;
using GruersShop.Data.Seeding;
using GruersShop.Services.Core.Service.Implementations.Account;
using GruersShop.Services.Core.Service.Interfaces.Account;
using GruersShop.Web.Infrastructure.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Connection string 
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Add DbContext 
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));



// Add HttpClient factory for services that need to make HTTP requests (like IPreviewService)
builder.Services.AddHttpClient();

// Add Identity 
builder.Services.AddDefaultIdentity<AppUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<AppDbContext>();

// Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy =>
        policy.RequireRole("Admin"));

    options.AddPolicy("ManagerPolicy", policy =>
       policy.RequireRole("Manager"));
});

// Repositories & Services
builder.Services.RegisterRepositories(typeof(IAppUserRepository).Assembly);
builder.Services.RegisterServices(typeof(IAccountService).Assembly);
builder.Services.AddScoped<IAccountService, AccountService>();

// Add MVC with custom error handling
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Configure error handling
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

var app = builder.Build();

// Seed Roles, Users and Data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    // Apply migrations first
    await context.Database.MigrateAsync();

    // Seed Identity (Roles and Users) - idempotent (safe to run multiple times)
    await IdentitySeeder.SeedRolesAsync(roleManager);
    await IdentitySeeder.SeedAdminAsync(userManager);
    await IdentitySeeder.SeedManagerAsync(userManager);

    //    // Seed Catalog Data - ONLY if Categories table is empty
    //    var anyCategories = await context.Categories.AnyAsync();
    //    if (!anyCategories)
    //    {
    //        try
    //        {
    //            await DbSeeder.SeedCategoriesAsync(context);
    //            await DbSeeder.SeedCatalogAsync(context);
    //            Console.WriteLine("✅ Database seeding completed successfully!");
    //        }
    //        catch (Exception ex)
    //        {
    //            Console.WriteLine($"❌ Seeding failed: {ex.Message}");
    //            throw;
    //        }
    //    }
    //    else
    //    {
    //        Console.WriteLine("📦 Categories already exist. Skipping catalog data seeding.");
    //    }
    //}

}

    // Static files with .glb support for 3D models
    var provider = new FileExtensionContentTypeProvider();
    provider.Mappings[".glb"] = "model/gltf-binary";

    // Configure error handling middleware
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error/500");
        app.UseHsts();
    }
    else
    {
        app.UseDeveloperExceptionPage();
    }

    app.UseStatusCodePagesWithReExecute("/Error/{0}");
    app.UseHttpsRedirection();

    app.UseStaticFiles(new StaticFileOptions
    {
        ContentTypeProvider = provider
    });

    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();

    // Custom error handling for 404
    app.Use(async (context, next) =>
    {
        await next();
        if (context.Response.StatusCode == 404 && !context.Response.HasStarted)
        {
            context.Items["originalPath"] = context.Request.Path;
            context.Request.Path = "/Error/404";
            await next();
        }
    });

    // Routing
    app.MapControllerRoute(
        name: "areas",
        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    app.MapRazorPages();

    await app.RunAsync();


