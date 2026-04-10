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

    // Favorite operations (Client)
    Task<bool> AddToFavoritesAsync(string userId, Guid productId);
    Task<bool> RemoveFromFavoritesAsync(string userId, Guid productId);
    Task<IEnumerable<ProductViewModel>> GetUserFavoritesAsync(string userId);
    Task<bool> IsFavoritedAsync(string userId, Guid productId);

    // Review operations (Client)
    Task<bool> AddReviewAsync(string userId, Guid productId, int rating, string? comment);
    Task<ReviewViewModel?> GetUserReviewAsync(string userId, Guid productId);
}