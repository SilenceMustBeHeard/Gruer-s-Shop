using GruersShop.Data.Models.Interactions;

namespace GruersShop.Data.Repositories.Interfaces.Interactions;

public interface IReviewManagementRepository : IReviewRepository
{
    Task<IEnumerable<Review>> GetAllActiveAsync();

    Task<IEnumerable<Review>> GetAllForAdminAsync();

    Task ToggleReviewStatusAsync(Review review);

    Task<Review?> GetByIdIncludingDeletedAsync(Guid id);

    Task<bool> HardDeleteReviewAsync(Guid id);
}