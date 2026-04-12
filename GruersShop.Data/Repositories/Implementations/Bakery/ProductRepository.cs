using GruersShop.Data.Models.Products;
using GruersShop.Data.Repositories.Implementations.Base;
using GruersShop.Data.Repositories.Interfaces.Bakery;
using Microsoft.EntityFrameworkCore;

namespace GruersShop.Data.Repositories.Implementations.Bakery;

public class ProductRepository : RepositoryAsync<Product, Guid>, IProductRepository
{
    public ProductRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Product?> GetProductWithReviewsAsync(Guid productId)
    {
        return await _dbSet
            .Include(p => p.Category)
            .Include(p => p.Reviews)
                .ThenInclude(r => r.User)
            .FirstOrDefaultAsync(p => p.Id == productId && !p.IsDeleted);
    }

    public async Task<bool> HasUserReviewedAsync(string userId, Guid productId)
    {
        return await _dbSet
            .Where(p => p.Id == productId)
            .SelectMany(p => p.Reviews)
            .AnyAsync(r => r.UserId == userId && !r.IsDeleted);
    }
}