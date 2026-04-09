using GruersShop.Data.Models.Base;
using System.ComponentModel.DataAnnotations;
using GruersShop.Data.Models.Products;

namespace GruersShop.Data.Models.Catalog;

public class Category : BaseDeletableEntity
{
    [Required]
    [MaxLength(100)]
    [MinLength(3)]
    public string Name { get; set; } = null!;

    [MaxLength(500)]
    public string? Description { get; set; }

    public string? IconClass { get; set; }  
    public int DisplayOrder { get; set; } = 0;


    public Guid CatalogId { get; set; }
    public virtual Catalog Catalog { get; set; } = null!;

    // Navigation properties
    public virtual ICollection<Product> Products { get; set; } 
        = new HashSet<Product>();
}