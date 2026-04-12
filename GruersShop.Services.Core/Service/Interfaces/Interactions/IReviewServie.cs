using GruersShop.Web.ViewModels.Interactions;
using GruersShop.Web.ViewModels.Products;

namespace GruersShop.Services.Core.Service.Interfaces.Interactions;

public interface IReviewService 
{

    Task AddReviewAsync(string userId, AddReviewViewModel model);

    Task<bool> HasUserReviewedAsync(string userId, Guid productId);


    Task<IEnumerable<AddReviewViewModel>> GetReviewsByProductIdAsync(Guid productId);
    Task<ProductViewModel?> GetWriteReviewModelAsync(string userId, Guid productId);

    Task<(bool Success, string? Error)> CreateReviewAsync(string userId, AddReviewViewModel model);

}