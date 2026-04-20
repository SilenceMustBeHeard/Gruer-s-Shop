using GruersShop.Data.Models.Products;
using GruersShop.Data.Repositories.Interfaces.CRUD;

namespace GruersShop.Data.Repositories.Interfaces.Bakery;

public interface IProductRepository : IFullRepositoryAsync<Product, Guid>
{
    Task<Product?> GetProductWithReviewsAsync(Guid productId);
    Task<bool> HasUserReviewedAsync(string userId, Guid productId);
    
    Task<Product?> GetByIdIncludingDeletedAsync(Guid id);
    Task<IEnumerable<Product>> GetAllActiveAsync();
    Task<IEnumerable<Product>> GetAllForAdminAsync();
    Task<IEnumerable<Product>> GetByCategoryIdAsync(Guid categoryId);
    Task<IEnumerable<Product>> GetPagedActiveProductsAsync(int page, int pageSize);
    Task<int> CountActiveAsync();

}