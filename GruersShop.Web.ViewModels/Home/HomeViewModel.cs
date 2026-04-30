using GruersShop.Data.Models.Catalog;
using GruersShop.Data.Models.Interactions;
using GruersShop.Data.Models.Products;

namespace GruersShop.Web.ViewModels.Home
{
    public class HomeViewModel
    {
        public List<Product> TopRatedProducts { get; set; } = new();
        public List<Category> Categories { get; set; } = new();
    }
}