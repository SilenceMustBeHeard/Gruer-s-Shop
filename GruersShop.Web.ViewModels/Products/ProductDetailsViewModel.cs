using GruersShop.Web.ViewModels.Interactions;
using System;
using System.Collections.Generic;
using System.Text;

namespace GruersShop.Web.ViewModels.Products;

public class ProductDetailsViewModel
{
    public ProductViewModel Product { get; set; } = null!;
    public bool UserHasReviewed { get; set; }
    public ReviewViewModel? UserReview { get; set; }
    public int NewRating { get; set; } = 5;
    public string? NewComment { get; set; }
    public IEnumerable<ProductViewModel> RelatedProducts { get; set; } 
        = new List<ProductViewModel>();
}