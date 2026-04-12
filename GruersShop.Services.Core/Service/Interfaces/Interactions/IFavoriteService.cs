using GruersShop.Web.ViewModels.Products;
using System;
using System.Collections.Generic;
using System.Text;

namespace GruersShop.Services.Core.Service.Interfaces.Interactions;

public interface IFavoriteService
{


    Task<IEnumerable<ProductViewModel>> GetUserFavoritesAsync(string userId);
    Task<bool> ToggleFavoriteAsync(string userId, Guid productId);
    Task<bool> IsFavoriteAsync(string userId, Guid productId);
}