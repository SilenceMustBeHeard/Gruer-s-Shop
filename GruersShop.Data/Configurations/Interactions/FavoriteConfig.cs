using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GruersShop.Data.Models.Interactions;

namespace GruersShop.Data.Configurations.Interactions;

public class FavoriteConfig : IEntityTypeConfiguration<Favorite>
{
    public void Configure(EntityTypeBuilder<Favorite> builder)
    {


        // Composite unique index - one user can favorite a product only once
        builder.HasIndex(f => new { f.UserId, f.ProductId })
            .IsUnique();

        // Relationships
        builder.HasOne(f => f.User)
            .WithMany(u => u.Favorites)
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(f => f.Product)
            .WithMany(p => p.FavoritedBy)
            .HasForeignKey(f => f.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        
    }
}