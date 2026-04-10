using GruersShop.Web.ViewModels.Products;
using System;
using System.Collections.Generic;
using System.Text;

namespace GruersShop.Web.ViewModels.Bakery;

public class CatalogIndexViewModel
{
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int TotalProducts { get; set; }
    public Guid? SelectedCategoryId { get; set; }
    public string? SearchTerm { get; set; }

    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;
    public IEnumerable<ProductViewModel> Products { get; set; } 
        = new List<ProductViewModel>();

    public IEnumerable<CategoryNavViewModel> Categories { get; set; } 
        = new List<CategoryNavViewModel>();
}