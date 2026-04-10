using GruersShop.Data.Models.Base;
using GruersShop.Data.Models.Catalog;
using GruersShop.Data.Models.Interactions;
using System.ComponentModel.DataAnnotations;

namespace GruersShop.Data.Models.Products;

public class Product : BaseDeletableEntity
{
    [Required]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 50 characters.")]
    public string Name { get; set; } = null!;

    [Required]
    [StringLength(500, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 500 characters.")]
    public string Description { get; set; } = null!;

    [Required]
    [Range(0.01, 40.00, ErrorMessage = "Price must be between 0.01 and 40.00.")]
    public decimal Price { get; set; }

    [Required]
    public string ImageUrl { get; set; } = null!;

    public bool IsAvailable { get; set; } = true;
    public int StockQuantity { get; set; } = 0;
    public double AverageRating { get; set; } = 0;

    // Foreign keys
    public Guid CategoryId { get; set; }

    // Navigation properties
    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<ProductIngredient> ProductIngredients { get; set; }
        = new HashSet<ProductIngredient>();

    public virtual ICollection<Favorite> FavoritedBy { get; set; }
        = new HashSet<Favorite>();

    public virtual ICollection<Review> Reviews { get; set; }
        = new HashSet<Review>();

    public virtual ICollection<OrderItem> OrderItems { get; set; } 
        = new HashSet<OrderItem>();
}