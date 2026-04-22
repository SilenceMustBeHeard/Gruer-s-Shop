
using Microsoft.EntityFrameworkCore;
using GruersShop.Data.Models.Interactions;
using GruersShop.Data.Repositories.Interfaces.Bakery;
using GruersShop.Data.Repositories.Interfaces.Interactions;
using GruersShop.Services.Core.Service.Admin.Interfaces.Interactions;
using GruersShop.Web.ViewModels.Interactions;
using GruersShop.Web.ViewModels.Products;

namespace GruersShop.Services.Core.Service.Admin.Implementations.Interactions;

public class ReviewManagementService : IReviewManagementService
{



    private readonly IReviewManagementRepository _reviewRepo;
    private readonly IProductRepository _productRepo;

    public ReviewManagementService(
        IReviewManagementRepository reviewRepo,
        IProductRepository productRepo)
    {
        _reviewRepo = reviewRepo;
        _productRepo = productRepo;
    }





    public async Task<bool> AddReviewAsync(string userId, Guid productId, int rating, string? comment)
    {
        if (await HasUserReviewedAsync(userId, productId))
            return false;

        var review = new Review
        {
            ProductId = productId,
            UserId = userId,
            Rating = rating,
            Comment = comment
        };

        await _reviewRepo.AddAsync(review);
        await _reviewRepo.SaveChangesAsync();

        return true;
    }

    public async Task<(bool Success, string? Error)> CreateReviewAsync(string userId, AddReviewViewModel model)
    {
        if (await HasUserReviewedAsync(userId, model.ProductId))
            return (false, "You have already reviewed this product.");

        var review = new Review
        {
            ProductId = model.ProductId,
            UserId = userId,
            Rating = model.Rating,
            Comment = model.Comment
        };

        await _reviewRepo.AddAsync(review);
        await _reviewRepo.SaveChangesAsync();

        return (true, null);
    }

    public async Task<bool> HasUserReviewedAsync(string userId, Guid productId)
    {
        return await _reviewRepo.HasUserReviewedAsync(userId, productId);
    }

    public async Task<IEnumerable<ReviewViewModel>> GetReviewsByProductIdAsync(Guid productId)
    {
        var reviews = await _reviewRepo.GetReviewsByProductIdAsync(productId);

        return reviews.Select(r => new ReviewViewModel
        {
            Id = r.Id,
            UserName = r.User?.FullName ?? "Anonymous",
            Rating = r.Rating,
            Comment = r.Comment,
            CreatedAt = r.CreatedAt
        });
    }

    public async Task<ProductViewModel?> GetWriteReviewModelAsync(string userId, Guid productId)
    {
        if (await HasUserReviewedAsync(userId, productId))
            return null;

        var product = await _productRepo.GetProductWithReviewsAsync(productId);
        if (product == null)
            return null;

        return new ProductViewModel
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            ImageUrl = product.ImageUrl,
            Price = product.Price,
            CategoryId = product.CategoryId,
            CategoryName = product.Category?.Name ?? string.Empty,
            AverageRating = product.Reviews.Any() ? product.Reviews.Average(r => r.Rating) : 0,
            ReviewCount = product.Reviews.Count,
            StockQuantity = product.StockQuantity
        };
    }


    public async Task<IEnumerable<ReviewViewModel>> GetUserReviewsAsync(string userId)
    {
        var reviews = await _reviewRepo
            .Query()
            .Where(r => r.UserId == userId && !r.IsDeleted)
            .Include(r => r.Product)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return reviews.Select(r => new ReviewViewModel
        {
            Id = r.Id,
            UserName = r.User?.FullName ?? "Anonymous",
            Rating = r.Rating,
            Comment = r.Comment,
            CreatedAt = r.CreatedAt,
            ProductName = r.Product?.Name ?? "Unknown Product"
        });
    }
    // Gets all active reviews (not deleted)
    public async Task<IEnumerable<ReviewViewModelList>> GetAllActiveAsync()
    {
        var reviews = await _reviewRepo
            .Query()
            .Where(r => !r.IsDeleted)  
            .Include(r => r.User)
            .Include(r => r.Product)
            .Select(r => new ReviewViewModelList
            {
                Id = r.Id,
                ProductId = r.ProductId,
                ProductName = r.Product != null ? r.Product.Name : "Unknown",
                UserId = r.UserId,
                UserName = r.User != null ? r.User.UserName ?? "Unknown" : "Unknown",
                UserEmail = r.User != null ? r.User.Email ?? "Unknown" : "Unknown",
                Rating = r.Rating,
                Comment = r.Comment ?? string.Empty,
                CreatedAt = r.CreatedAt,
                IsDeleted = r.IsDeleted
            })
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return reviews;
    }
    // Gets all reviews including deleted ones
    public async Task<IEnumerable<ReviewViewModelList>> GetAllIncludingDeletedAsync()
    {
        var reviews = await _reviewRepo
            .Query()
            .IgnoreQueryFilters()
            .Include(r => r.User)
            .Include(r => r.Product)
            .Select(r => new ReviewViewModelList
            {
                Id = r.Id,
                ProductId = r.ProductId,
                ProductName = r.Product != null ? r.Product.Name : "Unknown",
                UserId = r.UserId,
                UserName = r.User != null ? r.User.UserName ?? "Unknown" : "Unknown",
                UserEmail = r.User != null ? r.User.Email ?? "Unknown" : "Unknown",
                Rating = r.Rating,
                Comment = r.Comment ?? string.Empty,
                CreatedAt = r.CreatedAt,
                IsDeleted = r.IsDeleted
            })
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return reviews;
    }

    // Gets a specific review by ID
    public async Task<ReviewViewModelList?> GetByIdAsync(Guid id)
    {
        return await _reviewRepo
            .Query()
            .IgnoreQueryFilters()
            .Where(r => r.Id == id)
            .Include(r => r.User)
            .Include(r => r.Product)
            .Select(r => new ReviewViewModelList
            {
                Id = r.Id,
                ProductId = r.ProductId,
                ProductName = r.Product != null ? r.Product.Name : "Unknown",
                UserId = r.UserId,
                UserName = r.User != null ? r.User.UserName ?? "Unknown" : "Unknown",
                UserEmail = r.User != null ? r.User.Email ?? "Unknown" : "Unknown",
                Rating = r.Rating,
                Comment = r.Comment ?? string.Empty,
                CreatedAt = r.CreatedAt,
                IsDeleted = r.IsDeleted
            })
            .FirstOrDefaultAsync();
    }

    // Gets total count of active reviews
    public async Task<int> GetTotalActiveReviewsAsync()
    {
        return await _reviewRepo
            .Query()
            .Where(r => !r.IsDeleted)
            .CountAsync();
    }

    // Toggles the active/deleted status of a review (soft delete/restore)
    public async Task ToggleReviewAsync(Guid id)
    {
        var review = await _reviewRepo
            .Query()
            .IgnoreQueryFilters() // Important to find deleted reviews too
            .FirstOrDefaultAsync(r => r.Id == id);

        if (review == null)
        {
            throw new Exception($"Review with ID {id} not found");
        }

        // Toggle the IsDeleted status
        review.IsDeleted = !review.IsDeleted;
        review.UpdatedAt = DateTime.UtcNow;

        await _reviewRepo.UpdateAsync(review);
        await _reviewRepo.SaveChangesAsync();
    }

    // Permanently deletes a review (hard delete - use with caution)
    //public async Task<bool> HardDeleteReviewAsync(Guid id)
    //{
    //    var review = await _reviewRepo.GetByIdIncludingDeletedAsync(id);

    //    if (review == null)
    //        return false;

    //    _reviewRepo.Remove(review);
    //    await _reviewRepo.SaveChangesAsync();
    //    return true;
    //}

    // Gets reviews by user ID
    public async Task<IEnumerable<ReviewViewModelList>> GetReviewsByUserIdAsync(string userId)
    {
        return await _reviewRepo
            .Query()
            .Where(r => r.UserId == userId && !r.IsDeleted)
            .Include(r => r.Product)
            .Select(r => new ReviewViewModelList
            {
                Id = r.Id,
                ProductId = r.ProductId,
                ProductName = r.Product != null ? r.Product.Name : "Unknown",
                UserId = r.UserId,
                UserName = r.User != null ? r.User.UserName ?? "Unknown" : "Unknown",
                UserEmail = r.User != null ? r.User.Email ?? "Unknown" : "Unknown",
                Rating = r.Rating,
                Comment = r.Comment ?? string.Empty,
                CreatedAt = r.CreatedAt,
                IsDeleted = r.IsDeleted
            })
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    // Gets reviews by product ID with full details
    public async Task<IEnumerable<ReviewViewModelList>> GetDetailedReviewsByProductIdAsync(Guid productId)
    {
        return await _reviewRepo
            .Query()
            .Where(r => r.ProductId == productId && !r.IsDeleted)
            .Include(r => r.User)
            .Include(r => r.Product)
            .Select(r => new ReviewViewModelList
            {
               Id = r.Id,

            })
            .OrderByDescending(r => r.Rating)
            .ThenByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    //// Updates an existing review
    //public async Task<(bool Success, string? Error)> UpdateReviewAsync(Guid reviewId, string userId, int rating, string? comment)
    //{
    //    var review = await _reviewRepo.GetByIdAsync(reviewId);

    //    if (review == null)
    //        return (false, "Review not found");

    //    if (review.UserId != userId)
    //        return (false, "You can only edit your own reviews");

    //    review.Rating = rating;
    //    review.Comment = comment ?? string.Empty;


    //    _reviewRepo.Update(review);
    //    await _reviewRepo.SaveChangesAsync();

    //    return (true, null);
    //}

    // Gets average rating for a product
    public async Task<double> GetAverageRatingForProductAsync(Guid productId)
    {
        var reviews = await _reviewRepo
            .Query()
            .Where(r => r.ProductId == productId && !r.IsDeleted)
            .ToListAsync();

        return reviews.Any() ? reviews.Average(r => r.Rating) : 0;
    }

    // Gets review count for a product
    public async Task<int> GetReviewCountForProductAsync(Guid productId)
    {
        return await _reviewRepo
            .Query()
            .Where(r => r.ProductId == productId && !r.IsDeleted)
            .CountAsync();
    }

}
