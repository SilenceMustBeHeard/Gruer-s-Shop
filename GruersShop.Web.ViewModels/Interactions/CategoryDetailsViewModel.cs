using GruersShop.Web.ViewModels.Products;
using System;
using System.Collections.Generic;
using System.Text;

namespace GruersShop.Web.ViewModels.Interactions;

public class CategoryDetailsViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? IconClass { get; set; }
    public IEnumerable<ProductViewModel> Products { get; set; } 
        = new List<ProductViewModel>();

    public int TotalProducts { get; set; }
}