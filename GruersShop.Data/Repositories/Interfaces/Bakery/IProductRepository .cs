using GruersShop.Data.Models.Products;
using GruersShop.Data.Repositories.Interfaces.CRUD;

namespace GruersShop.Data.Repositories.Interfaces.Bakery;

public interface IProductRepository : IFullRepositoryAsync<Product, Guid>
{
    Task<Product?> GetProductWithReviewsAsync(Guid productId);
    Task<bool> HasUserReviewedAsync(string userId, Guid productId);
}