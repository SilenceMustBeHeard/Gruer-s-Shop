using GruersShop.Data.Models.Interactions;
using GruersShop.Data.Repositories.Implementations.Base;
using GruersShop.Data.Repositories.Interfaces.CRUD;
using System;
using System.Collections.Generic;
using System.Text;

namespace GruersShop.Data.Repositories.Interfaces.Interactions;

public interface IFavoriteRepository
    :IFullRepositoryAsync<Favorite, Guid>
{

    Task<Favorite?> GetByCompositeKeyAsync(string userId, Guid productId);

    Task<bool> ExistsAsync(string userId, Guid productId);
}