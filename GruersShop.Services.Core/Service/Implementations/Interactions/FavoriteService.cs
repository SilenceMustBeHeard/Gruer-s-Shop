using GruersShop.Data.Models.Interactions;
using GruersShop.Data.Repositories.Interfaces.Interactions;
using GruersShop.Services.Core.Service.Interfaces.Interactions;
using GruersShop.Web.ViewModels.Products;
using Microsoft.EntityFrameworkCore;

namespace GruersShop.Services.Core.Service.Implementations.Interactions;

public class FavoriteService : IFavoriteService
{
    private readonly IFavoriteRepository _favoriteRepository;

    public FavoriteService(IFavoriteRepository favoriteRepository)
    {
        _favoriteRepository = favoriteRepository;
    }

    public async Task<bool> ToggleFavoriteAsync(string userId, Guid productId)
    {
        var favorite = await _favoriteRepository.GetByCompositeKeyAsync(userId, productId);

        if (favorite == null)
        {
            // Add new favorite
            await _favoriteRepository.AddAsync(new Favorite
            {
                UserId = userId,
                ProductId = productId
            });
            await _favoriteRepository.SaveChangesAsync();
            return true; // Now favorited
        }

        // Toggle soft delete
        favorite.IsDeleted = !favorite.IsDeleted;
        await _favoriteRepository.UpdateAsync(favorite);
        await _favoriteRepository.SaveChangesAsync();

        return !favorite.IsDeleted; // Returns true if now favorited, false if removed
    }
    public async Task<IEnumerable<ProductViewModel>> GetUserFavoritesAsync(string userId)
    {
        var favorites = await _favoriteRepository
            .Query()
            .Where(f => f.UserId == userId && !f.IsDeleted)
            .Include(f => f.Product)
                .ThenInclude(p => p.Category)
            .ToListAsync();

        return favorites.Select(f => new ProductViewModel
        {
            Id = f.Product.Id,
            Name = f.Product.Name,
            Description = f.Product.Description,
            ImageUrl = f.Product.ImageUrl,
            Price = f.Product.Price,
            CategoryId = f.Product.CategoryId,
            CategoryName = f.Product.Category?.Name ?? string.Empty,
            IsFavorited = true,
            AverageRating = f.Product.AverageRating,
            ReviewCount = f.Product.Reviews?.Count ?? 0,
            StockQuantity = f.Product.StockQuantity
        });
    }
    public async Task<bool> IsFavoriteAsync(string userId, Guid productId)
    {
        return await _favoriteRepository.ExistsAsync(userId, productId);
    }
}