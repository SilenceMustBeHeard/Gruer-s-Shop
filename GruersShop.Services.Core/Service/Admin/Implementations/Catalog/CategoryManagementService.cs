using GruersShop.Services.Core.Service.Admin.Interfaces.Catalog;
using GruersShop.Services.Core.Service.Interfaces.Bakery;
using GruersShop.Web.ViewModels.Admin.Category;
using GruersShop.Web.ViewModels.Bakery;
using GruersShop.Web.ViewModels.Interactions;
using System;
using System.Collections.Generic;
using System.Text;

namespace GruersShop.Services.Core.Service.Admin.Implementations.Catalog;

public class CategoryManagementService : ICategoryManagementService
{
    public async Task AddCategoryAsync(CategoryViewModelCreate model)
    {
        throw new NotImplementedException();
    }

    public async Task EditCategoryAsync(Guid id, CategoryViewModelEdit model)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<CategoryNavViewModel>> GetAllActiveCategoriesAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<CategoryViewModelList>> GetAllActiveCategoriesForBulkEditAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<CategoryViewModelList>> GetAllCategoriesForAdminAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<CategoryNavViewModel>> GetCategoriesWithProductCountAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<CategoryNavViewModel?> GetCategoryByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<CategoryViewModelEdit?> GetCategoryForEditByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<CategoryDetailsViewModel?> GetCategoryWithProductsAsync(Guid id, string? userId)
    {
        throw new NotImplementedException();
    }

    public async Task ToggleCategoryAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}
