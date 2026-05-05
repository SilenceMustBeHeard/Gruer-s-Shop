
using GruersShop.Data;
using GruersShop.Data.Models.Base;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GruersShop.API.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Connection string

            var connectionString = builder.Configuration
                .GetConnectionString("DefaultConnection")
          ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            // DbContext
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));



            // Identity (Cookie based – same as MVC)

            builder.Services
                .AddIdentityCore<AppUser>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddSignInManager();


            // Authentication + Authorization

            builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme)
                .AddIdentityCookies();

            builder.Services.AddAuthorization();

            // Controllers + OpenAPI

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // HTTP pipeline
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();


            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
