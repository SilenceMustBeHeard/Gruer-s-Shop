using GruersShop.Web.ViewModels.Products;

namespace GruersShop.Web.ViewModels.Interactions;

public class FavoriteListViewModel
{
    public int TotalFavorites { get; set; }

    public IEnumerable<ProductViewModel> FavoriteProducts { get; set; }
        = new List<ProductViewModel>();
}