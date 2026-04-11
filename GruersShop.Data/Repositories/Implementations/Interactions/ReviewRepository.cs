using GruersShop.Data.Models.Interactions;
using GruersShop.Data.Repositories.Implementations.Base;
using GruersShop.Data.Repositories.Interfaces.Interactions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GruersShop.Data.Repositories.Implementations.Interactions;

public class ReviewRepository :
      RepositoryAsync<Review, Guid>, IReviewRepository
{
    public ReviewRepository(AppDbContext context) :
        base(context)
    {
    }

    // Check if a user has already reviewed a product
    public async Task<bool> HasUserReviewedAsync(string userId, Guid productId)
    {
        return await _dbSet
            .AnyAsync(r => r.UserId == userId && r.ProductId == productId);
    }


    // Get all reviews for a specific product
    public async Task<IEnumerable<Review>> GetReviewsByProductIdAsync(Guid productId)
    {

        return await _dbSet
            .Include(r => r.Product)
            .Where(r => r.ProductId == productId)
            .ToListAsync();
    }
}