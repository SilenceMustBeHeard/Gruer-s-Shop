using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GruersShop.Data.Models.Catalog;

namespace GruersShop.Data.Configurations.CatalogItems;

public class CategoryConfig : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Description)
            .HasMaxLength(500);

        builder.Property(c => c.IconClass)
            .HasMaxLength(50);

        builder.Property(c => c.DisplayOrder)
            .HasDefaultValue(0);

        // Indexes
        builder.HasIndex(c => c.Name)
            .IsUnique();

        builder.HasIndex(c => c.DisplayOrder);

        // Soft delete filter
        builder.HasQueryFilter(c => !c.IsDeleted);
    }
}