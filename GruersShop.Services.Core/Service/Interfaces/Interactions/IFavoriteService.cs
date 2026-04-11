using System;
using System.Collections.Generic;
using System.Text;

namespace GruersShop.Services.Core.Service.Interfaces.Interactions;

public interface IFavoriteService
{



    Task<bool> ToggleFavoriteAsync(string userId, Guid productId);
    Task<bool> IsFavoriteAsync(string userId, Guid productId);
}