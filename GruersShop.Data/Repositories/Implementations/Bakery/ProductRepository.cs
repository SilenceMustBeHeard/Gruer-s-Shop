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
        => await _dbSet
            .Include(p => p.Category)
            .Include(p => p.Reviews)
                .ThenInclude(r => r.User)
            .FirstOrDefaultAsync(p => p.Id == productId && !p.IsDeleted);

    public async Task<bool> HasUserReviewedAsync(string userId, Guid productId)
        => await _dbSet
            .Where(p => p.Id == productId)
            .SelectMany(p => p.Reviews)
            .AnyAsync(r => r.UserId == userId && !r.IsDeleted);
    



    public async Task<Product?> GetByIdIncludingDeletedAsync(Guid id)
    
        => await _dbSet
            .IgnoreQueryFilters()
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);
    

    public async Task<Product?> GetByIdWithDetailsAsync(Guid id)
    
        => await _dbSet
            .Include(p => p.Category)
            .Include(p => p.ProductIngredients)
                .ThenInclude(pi => pi.Ingredient)
            .Include(p => p.Reviews)
                .ThenInclude(r => r.User)
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
    

    public async Task<IEnumerable<Product>> GetAllActiveAsync()
        => await _dbSet
            .Where(p => !p.IsDeleted && p.IsAvailable)
            .Include(p => p.Category)
            .OrderBy(p => p.Name)
            .ToListAsync();
    

    public async Task<IEnumerable<Product>> GetAllForAdminAsync()
    
        => await _dbSet
            .IgnoreQueryFilters()
            .Include(p => p.Category)
            .OrderBy(p => p.Name)
            .ToListAsync();
    

    public async Task<IEnumerable<Product>> GetByCategoryIdAsync(Guid categoryId)
    
        => await _dbSet
            .Where(p => p.CategoryId == categoryId && !p.IsDeleted && p.IsAvailable)
            .Include(p => p.Category)
            .OrderBy(p => p.Name)
            .ToListAsync();
    

    public async Task<IEnumerable<Product>> GetPagedActiveProductsAsync(int page, int pageSize)
    
        => await _dbSet
            .Where(p => !p.IsDeleted && p.IsAvailable)
            .Include(p => p.Category)
            .OrderBy(p => p.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    

    public async Task<int> CountActiveAsync()
    
        => await _dbSet
            .CountAsync(p => !p.IsDeleted && p.IsAvailable);
    
}