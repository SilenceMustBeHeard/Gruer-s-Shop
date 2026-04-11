using GruersShop.Data.Models.Interactions;
using GruersShop.Data.Repositories.Implementations.Base;
using GruersShop.Data.Repositories.Interfaces.Interactions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GruersShop.Data.Repositories.Implementations.Interactions;

public class FavoriteRepository :
         RepositoryAsync<Favorite, Guid>,
         IFavoriteRepository    
{
    public FavoriteRepository(AppDbContext context)
        : base(context)
    {
    }

    public async Task<Favorite?> GetByCompositeKeyAsync(string userId, Guid productId)


        => await _dbSet
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(f =>
                f.UserId == userId &&
               f.ProductId == productId);



    public async Task<bool> ExistsAsync(string userId, Guid productId)
        => await _dbSet
            .AnyAsync(f =>
                f.UserId == userId &&
                f.ProductId == productId &&
                !f.IsDeleted);
}