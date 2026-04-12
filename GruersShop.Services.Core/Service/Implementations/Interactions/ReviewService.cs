using GruersShop.Data.Models.Interactions;
using GruersShop.Data.Repositories.Interfaces.Bakery;
using GruersShop.Data.Repositories.Interfaces.Interactions;
using GruersShop.Services.Core.Service.Interfaces.Interactions;
using GruersShop.Web.ViewModels.Interactions;
using GruersShop.Web.ViewModels.Products;
using System;
using System.Collections.Generic;
using System.Text;

namespace GruersShop.Services.Core.Service.Implementations.Interactions;

public class ReviewService : IReviewService
{
    private readonly IReviewRepository _reviewRepo;
    private readonly ICatalogRepository _catalogRepo;
    private readonly IProductRepository _productRepo;

    public ReviewService(IReviewRepository reviewRepo,
            IProductRepository productRepo,
        ICatalogRepository catalogRepo)
    {
        _reviewRepo = reviewRepo;
        _reviewRepo = reviewRepo;
        _catalogRepo = catalogRepo;
    }

    // adds a new review for a catalog product by a user
    public async Task AddReviewAsync(string userId, AddReviewViewModel model)
    {
        var review = new Review
        {
            ProductId = model.ProductId,
            UserId = userId,
            Rating = model.Rating,
            Comment = model.Comment
        };

        await _reviewRepo.AddAsync(review);
        await _reviewRepo.SaveChangesAsync();
    }

    // checks if a user has already reviewed a specific catalog product
    public async Task<bool> HasUserReviewedAsync(string userId, Guid productId)
    {
        return await _reviewRepo.HasUserReviewedAsync(userId, productId);
    }

    // retrieves all reviews for a specific catalog product and maps them to view models
    //  allows the application to display reviews for a catalog product including the rating and comment left by users
    public async Task<IEnumerable<AddReviewViewModel>> GetReviewsByProductIdAsync(Guid productId)
    {
        var reviews = await _reviewRepo.GetReviewsByProductIdAsync(productId);

        return reviews.Select(r => new AddReviewViewModel
        {
            ProductId = r.ProductId,
            ProductName = r.Product.Name,
            Rating = r.Rating,
            Comment = r.Comment
        }).ToList();
    }

    // retrieves a catalog product and its review form
    public async Task<ProductViewModel?> GetWriteReviewModelAsync(string userId, Guid productId)
    {
        if (await _productRepo.HasUserReviewedAsync(userId, productId))
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
}