using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GruersShop.Data.Models.Catalog;

namespace GruersShop.Data.Configurations.CatalogItems;

public class CatalogConfig : IEntityTypeConfiguration<Catalog>
{
    public void Configure(EntityTypeBuilder<Catalog> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(c => c.Name)
            .IsUnique();

        builder.HasIndex(c => c.DisplayOrder);

        // Relationship: Catalog has many Categories
        builder.HasMany(c => c.Categories)
            .WithOne(cat => cat.Catalog)
            .HasForeignKey(cat => cat.CatalogId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(c => !c.IsDeleted);
    }
}