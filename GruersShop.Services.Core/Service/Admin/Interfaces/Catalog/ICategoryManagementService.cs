using GruersShop.Services.Core.Service.Interfaces.Bakery;
using GruersShop.Web.ViewModels.Admin.Category;
using System;
using System.Collections.Generic;
using System.Text;

namespace GruersShop.Services.Core.Service.Admin.Interfaces.Catalog;

public interface ICategoryManagementService : ICategoryClientService
{
    Task<IEnumerable<CategoryViewModelList>> GetAllActiveCategoriesForBulkEditAsync();

    Task AddCategoryAsync(CategoryViewModelCreate model);

    Task<CategoryViewModelEdit?> GetCategoryForEditByIdAsync(Guid id);

    Task EditCategoryAsync(Guid id, CategoryViewModelEdit model);

    Task ToggleCategoryAsync(Guid id);

    Task<IEnumerable<CategoryViewModelList>> GetAllCategoriesForAdminAsync();
}
