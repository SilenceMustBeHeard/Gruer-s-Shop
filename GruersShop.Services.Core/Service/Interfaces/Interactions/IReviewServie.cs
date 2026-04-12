using GruersShop.Web.ViewModels.Interactions;
using GruersShop.Web.ViewModels.Products;

namespace GruersShop.Services.Core.Service.Interfaces.Interactions;

public interface IReviewService 
{

    Task<bool> AddReviewAsync(string userId, Guid productId, int rating, string? comment);

    Task<(bool Success, string? Error)> CreateReviewAsync(string userId, AddReviewViewModel model);
    Task<bool> HasUserReviewedAsync(string userId, Guid productId);
    Task<IEnumerable<ReviewViewModel>> GetReviewsByProductIdAsync(Guid productId);

    Task<ProductViewModel?> GetWriteReviewModelAsync(string userId, Guid productId);

    Task<IEnumerable<ReviewViewModel>> GetUserReviewsAsync(string userId);

}