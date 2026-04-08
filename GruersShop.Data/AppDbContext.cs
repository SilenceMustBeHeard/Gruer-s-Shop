using GruersShop.Data.Models;
using GruersShop.Data.Models.Messages;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GruersShop.Data;

public class AppDbContext : IdentityDbContext<AppUser>
{
    // Parameterless constructor for migrations
    public AppDbContext()
    {
    }

    // Constructor for runtime DI
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    // ✅ ADD THIS METHOD - it's required for migrations
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Server=localhost;Database=GruersShop;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true");
        }
    }

    public virtual DbSet<SystemInboxMessage> SystemInboxMessages { get; set; } = null!;
    public virtual DbSet<ContactMessage> ContactMessages { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}