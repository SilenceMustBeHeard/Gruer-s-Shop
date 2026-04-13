using GruersShop.Services.Core.Service.Interfaces.Interactions;
using GruersShop.Web.ViewModels.Interactions;
using System;
using System.Collections.Generic;
using System.Text;

namespace GruersShop.Services.Core.Service.Admin.Interfaces.Interactions;

public interface IReviewManagementService : IReviewService
{
 
    // Query operations
    Task<IEnumerable<AddReviewViewModel>> GetReviewsByDesignIdAsync(Guid catalogDesignId);
    Task<IEnumerable<ReviewViewModelList>> GetDetailedReviewsByDesignIdAsync(Guid catalogDesignId);
    Task<IEnumerable<ReviewViewModelList>> GetReviewsByUserIdAsync(string userId);
    Task<ReviewViewModelList?> GetByIdAsync(Guid id);

    // Status management
    Task ToggleReviewAsync(Guid id);
    Task<IEnumerable<ReviewViewModelList>> GetAllActiveAsync();
    Task<IEnumerable<ReviewViewModelList>> GetAllIncludingDeletedAsync();


    // Statistics
    Task<int> GetTotalActiveReviewsAsync();
    Task<double> GetAverageRatingForProductAsync(Guid productId);
    Task<int> GetReviewCountForProductAsync(Guid productId);

 
}
