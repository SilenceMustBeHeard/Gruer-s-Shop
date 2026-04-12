using GruersShop.Data.Models.Interactions;
using GruersShop.Data.Repositories.Interfaces.Bakery;
using GruersShop.Data.Repositories.Interfaces.Interactions;
using GruersShop.Services.Core.Service.Interfaces.Interactions;
using GruersShop.Web.ViewModels.Interactions;
using GruersShop.Web.ViewModels.Products;
using Microsoft.EntityFrameworkCore;

namespace GruersShop.Services.Core.Service.Implementations.Interactions;

public class ReviewService : IReviewService
{
    private readonly IReviewRepository _reviewRepo;
    private readonly IProductRepository _productRepo;

    public ReviewService(
        IReviewRepository reviewRepo,
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
}