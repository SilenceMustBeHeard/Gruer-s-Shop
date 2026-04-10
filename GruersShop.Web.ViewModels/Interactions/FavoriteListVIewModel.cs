using GruersShop.Web.ViewModels.Products;
using System;
using System.Collections.Generic;
using System.Text;

namespace GruersShop.Web.ViewModels.Interactions;

public class FavoriteListViewModel
{
    public int TotalFavorites { get; set; }
        public IEnumerable<ProductViewModel> FavoriteProducts { get; set; } 
            = new List<ProductViewModel>();
}