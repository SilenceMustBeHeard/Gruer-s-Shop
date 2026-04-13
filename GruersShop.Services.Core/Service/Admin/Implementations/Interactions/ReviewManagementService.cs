

using GruersShop.Services.Core.Service.Admin.Interfaces.Interactions;
using GruersShop.Web.ViewModels.Interactions;
using GruersShop.Web.ViewModels.Products;

namespace GruersShop.Services.Core.Service.Admin.Implementations.Interactions;

public class ReviewManagementService : IReviewManagementService
{
    public Task<bool> AddReviewAsync(string userId, Guid productId, int rating, string? comment)
    {
        throw new NotImplementedException();
    }

    public Task<(bool Success, string? Error)> CreateReviewAsync(string userId, AddReviewViewModel model)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<ReviewViewModelList>> GetAllActiveAsync()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<ReviewViewModelList>> GetAllIncludingDeletedAsync()
    {
        throw new NotImplementedException();
    }

    public Task<double> GetAverageRatingForProductAsync(Guid productId)
    {
        throw new NotImplementedException();
    }

    public Task<ReviewViewModelList?> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<ReviewViewModelList>> GetDetailedReviewsByDesignIdAsync(Guid catalogDesignId)
    {
        throw new NotImplementedException();
    }

    public Task<int> GetReviewCountForProductAsync(Guid productId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<AddReviewViewModel>> GetReviewsByDesignIdAsync(Guid catalogDesignId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<ReviewViewModel>> GetReviewsByProductIdAsync(Guid productId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<ReviewViewModelList>> GetReviewsByUserIdAsync(string userId)
    {
        throw new NotImplementedException();
    }

    public Task<int> GetTotalActiveReviewsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<ReviewViewModel>> GetUserReviewsAsync(string userId)
    {
        throw new NotImplementedException();
    }

    public Task<ProductViewModel?> GetWriteReviewModelAsync(string userId, Guid productId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> HasUserReviewedAsync(string userId, Guid productId)
    {
        throw new NotImplementedException();
    }

    public Task ToggleReviewAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}
