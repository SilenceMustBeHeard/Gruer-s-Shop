using GruersShop.Data.Models.Interactions;
using GruersShop.Data.Repositories.Interfaces.Interactions;
using GruersShop.Services.Core.Service.Interfaces.Interactions;

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

    public async Task<bool> IsFavoriteAsync(string userId, Guid productId)
    {
        return await _favoriteRepository.ExistsAsync(userId, productId);
    }
}