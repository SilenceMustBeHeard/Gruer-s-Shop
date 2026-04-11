using GruersShop.Data.Models.Interactions;
using GruersShop.Data.Repositories.Interfaces.CRUD;
using GruersShop.Data.Repositories.Interfaces.Interactions;
using GruersShop.Services.Core.Service.Interfaces.Interactions;
using System;
using System.Collections.Generic;
using System.Text;

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
            await _favoriteRepository.AddAsync(new Favorite
            {
                UserId = userId,
                ProductId = productId,
                IsDeleted = false
            });


            return true;
        }
      

        favorite.IsDeleted = !favorite.IsDeleted;

        await _favoriteRepository.SaveChangesAsync();


        return !favorite.IsDeleted;
    }

    // Checks if a product is marked as a favorite by a user
    public async Task<bool> IsFavoriteAsync(string userId, Guid productId)
    {
        return await _favoriteRepository.ExistsAsync(userId, productId);
    }
}
