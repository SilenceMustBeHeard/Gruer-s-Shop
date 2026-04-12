using GruersShop.Web.ViewModels.Admin.Category;
using System.ComponentModel.DataAnnotations;

namespace GruersShop.Web.ViewModels.Admin.Products;

public class ProductViewModelEdit
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Product name is required")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 50 characters")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Description is required")]
    [StringLength(500, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 500 characters")]
    public string Description { get; set; } = null!;

    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, 40.00, ErrorMessage = "Price must be between 0.01 and 40.00")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Image URL is required")]
    [Url(ErrorMessage = "Please enter a valid URL")]
    public string ImageUrl { get; set; } = null!;

    [Required(ErrorMessage = "Category is required")]
    public Guid CategoryId { get; set; }

    public bool IsAvailable { get; set; }

    [Range(0, 999, ErrorMessage = "Stock quantity must be between 0 and 999")]
    public int StockQuantity { get; set; }

    public string? CurrentImageUrl { get; set; }
    public string? CategoryName { get; set; }

    // For dropdown
    public IEnumerable<CategorySelectViewModel>? Categories { get; set; }
}