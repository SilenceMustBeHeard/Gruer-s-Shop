using GruersShop.Web.ViewModels.Interactions;
using System;
using System.Collections.Generic;
using System.Text;

namespace GruersShop.Web.ViewModels.Products;

public class ProductViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string ImageUrl { get; set; } = null!;
    public decimal Price { get; set; }

    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = null!;
    public bool IsFavorited { get; set; }
    public double AverageRating { get; set; }
    public int ReviewCount { get; set; }
    public int StockQuantity { get; set; }
    public bool IsInStock => StockQuantity > 0;
    public string FormattedPrice => Price.ToString("C");
    public List<ReviewViewModel> Reviews { get; set; } = new();
}