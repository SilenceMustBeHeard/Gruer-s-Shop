using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GruersShop.Data.Models.Base;
using GruersShop.Data.Models.Catalog;
using GruersShop.Data.Models.Interactions;
using GruersShop.Data.Models.Messages;
using GruersShop.Data.Models.Products;

namespace GruersShop.Data;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }



    public virtual DbSet<Catalog> Catalogs { get; set; } = null!;
    public virtual DbSet<Product> Products { get; set; } = null!;
    public virtual DbSet<Category> Categories { get; set; } = null!;
    public virtual DbSet<Order> Orders { get; set; } = null!;
    public virtual DbSet<OrderItem> OrderItems { get; set; } = null!;
    public virtual DbSet<Favorite> Favorites { get; set; } = null!;
    public virtual DbSet<Review> Reviews { get; set; } = null!;
    public virtual DbSet<Ingredient> Ingredients { get; set; } = null!;
    public virtual DbSet<ProductIngredient> ProductIngredients { get; set; } = null!;
    public virtual DbSet<ContactMessage> ContactMessages { get; set; } = null!;
    public virtual DbSet<SystemInboxMessage> SystemInboxMessages { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Apply all configurations automatically
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Global soft delete filter
        builder.Entity<Catalog>().HasQueryFilter(c => !c.IsDeleted);
        builder.Entity<Product>().HasQueryFilter(p => !p.IsDeleted);
        builder.Entity<Category>().HasQueryFilter(c => !c.IsDeleted);
        builder.Entity<Order>().HasQueryFilter(o => !o.IsDeleted);
        builder.Entity<OrderItem>().HasQueryFilter(oi => !oi.IsDeleted);
        builder.Entity<Favorite>().HasQueryFilter(f => !f.IsDeleted);
        builder.Entity<Review>().HasQueryFilter(r => !r.IsDeleted);
        builder.Entity<ContactMessage>().HasQueryFilter(m => !m.IsDeleted);
        builder.Entity<SystemInboxMessage>().HasQueryFilter(m => !m.IsDeleted);
    }
}