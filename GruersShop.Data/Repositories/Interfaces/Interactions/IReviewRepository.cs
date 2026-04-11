using GruersShop.Data.Models.Interactions;
using GruersShop.Data.Repositories.Interfaces.CRUD;

namespace GruersShop.Data.Repositories.Interfaces.Interactions;

public interface IReviewRepository
      : IFullRepositoryAsync<Review, Guid>
{
    Task<bool> HasUserReviewedAsync(string userId, Guid productId);

    Task<IEnumerable<Review>> GetReviewsByProductIdAsync(Guid productId);
}