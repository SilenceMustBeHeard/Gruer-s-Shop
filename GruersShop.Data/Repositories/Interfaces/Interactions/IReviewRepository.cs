using GruersShop.Data.Models.Interactions;
using GruersShop.Data.Repositories.Interfaces.CRUD;

namespace GruersShop.Data.Repositories.Interfaces.Interactions;

public interface IReviewRepository
      : IFullRepositoryAsync<Review, Guid>
{
    Task<bool> HasUserReviewedAsync(string userId, Guid catalogDesignId);

    Task<IEnumerable<Review>> GetReviewsByDesignIdAsync(Guid catalogDesignId);
}