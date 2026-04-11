using GruersShop.Web.ViewModels.Bakery;
using GruersShop.Web.ViewModels.Interactions;
using GruersShop.Web.ViewModels.Products;
using System;
using System.Collections.Generic;
using System.Text;

namespace GruersShop.Services.Core.Service.Interfaces.Bakery;

public interface ICatalogClientService
{
    // Read operations
    Task<IEnumerable<ProductViewModel>> GetPublicCatalogAsync(string? userId, int page, int pageSize, bool isGuest);
    Task<ProductViewModel?> GetProductDetailsAsync(Guid id, string? userId);
    Task<IEnumerable<ProductViewModel>> GetProductsByCategoryAsync(Guid categoryId, string? userId);
    Task<int> GetTotalActiveProductsAsync();
    Task<IEnumerable<CategoryNavViewModel>> GetCategoriesForNavAsync();


}