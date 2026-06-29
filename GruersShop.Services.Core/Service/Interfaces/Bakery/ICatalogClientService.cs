using GruersShop.Web.ViewModels.Bakery;
using GruersShop.Web.ViewModels.Products;

namespace GruersShop.Services.Core.Service.Interfaces.Bakery;

public interface ICatalogClientService
{
    // Read operations
    Task<IEnumerable<ProductViewModel>> GetPublicCatalogAsync(string? userId, int page, int pageSize, bool isGuest, Guid? categoryId);

    Task<ProductViewModel?> GetProductDetailsAsync(Guid id, string? userId);

    Task<IEnumerable<ProductViewModel>> GetProductsByCategoryAsync(Guid categoryId, string? userId);

    Task<int> GetTotalActiveProductsAsync(Guid? categoryId);

    Task<IEnumerable<CategoryNavViewModel>> GetCategoriesForNavAsync();

    Task<ProductDetailsViewModel?> GetProductDetailsViewModelAsync(Guid id, string? userId);
}