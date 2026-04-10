using GruersShop.Web.ViewModels.Bakery;
using GruersShop.Web.ViewModels.Interactions;
using System;
using System.Collections.Generic;
using System.Text;

namespace GruersShop.Services.Core.Service.Interfaces.Bakery;


public interface ICategoryClientService
{
    Task<IEnumerable<CategoryNavViewModel>> GetAllActiveCategoriesAsync();
    Task<CategoryNavViewModel?> GetCategoryByIdAsync(Guid id);
    Task<IEnumerable<CategoryNavViewModel>> GetCategoriesWithProductCountAsync();
    Task<CategoryDetailsViewModel?> GetCategoryWithProductsAsync(Guid id, string? userId);
}