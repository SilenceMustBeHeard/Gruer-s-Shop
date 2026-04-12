using GruersShop.Services.Core.Service.Admin.Interfaces.Product;
using GruersShop.Services.Core.Service.Interfaces.Bakery;
using GruersShop.Web.ViewModels.Admin.Products;
using GruersShop.Web.ViewModels.Bakery;
using GruersShop.Web.ViewModels.Products;
using System;
using System.Collections.Generic;
using System.Text;

namespace GruersShop.Services.Core.Service.Admin.Implementations.Products;

public class ProductManagementService : IProductManagementService
{
    public async Task AddProductAsync(ProductViewModelCreate model)
    {
        throw new NotImplementedException();
    }

    public async Task EditProductAsync(Guid id, ProductViewModelEdit model)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<ProductViewModelList>> GetAllActiveProductsAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<ProductViewModelList>> GetAllProductsForAdminAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<CategoryNavViewModel>> GetCategoriesForNavAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<ProductViewModel?> GetProductDetailsAsync(Guid id, string? userId)
    {
        throw new NotImplementedException();
    }

    public async Task<ProductDetailsViewModel?> GetProductDetailsViewModelAsync(Guid id, string? userId)
    {
        throw new NotImplementedException();
    }

    public async Task<ProductViewModelEdit?> GetProductForEditByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<ProductViewModel>> GetProductsByCategoryAsync(Guid categoryId, string? userId)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<ProductViewModel>> GetPublicCatalogAsync(string? userId, int page, int pageSize, bool isGuest)
    {
        throw new NotImplementedException();
    }

    public async Task<int> GetTotalActiveProductsAsync()
    {
        throw new NotImplementedException();
    }

    public async Task ToggleProductAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}
