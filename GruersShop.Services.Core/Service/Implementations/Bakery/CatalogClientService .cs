using GruersShop.Data.Models.Catalog;
using GruersShop.Data.Models.Interactions;
using GruersShop.Data.Models.Products;
using GruersShop.Data.Repositories.Interfaces.CRUD;
using GruersShop.Services.Core.Service.Interfaces.Bakery;
using GruersShop.Web.ViewModels.Bakery;
using GruersShop.Web.ViewModels.Interactions;
using GruersShop.Web.ViewModels.Products;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GruersShop.Services.Core.Service.Implementations.Bakery;

public class CatalogClientService : ICatalogClientService
{
    private readonly IUnitOfWork _uow;

    public CatalogClientService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    // ============ READ OPERATIONS (Client has access) ============

    // Get all active products for catalog page
    public async Task<IEnumerable<ProductViewModel>> GetPublicCatalogAsync(
        string? userId,
        int page,
        int pageSize,
        bool isGuest)
    {
        var query = _uow.Repository<Product, Guid>()
            .Query()
            .Where(p => p.IsAvailable && !p.IsDeleted)
            .Include(p => p.Category)
            .Include(p => p.Reviews)
            .Include(p => p.FavoritedBy);

        if (isGuest)
        {
            page = 1;
            pageSize = 3;
        }

        return await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ProductViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                ImageUrl = p.ImageUrl,
                Price = p.Price,
                CategoryId = p.CategoryId,
                CategoryName = p.Category.Name,
                IsFavorited = userId != null && p.FavoritedBy.Any(f => f.UserId == userId && !f.IsDeleted),
                AverageRating = p.AverageRating,
                ReviewCount = p.Reviews.Count,
                StockQuantity = p.StockQuantity
            })
            .ToListAsync();
    }

    // Get single product details
    public async Task<ProductViewModel?> GetProductDetailsAsync(Guid id, string? userId)
    {
        return await _uow.Repository<Product, Guid>()
            .Query()
            .Where(p => p.Id == id && p.IsAvailable && !p.IsDeleted)
            .Include(p => p.Category)
            .Include(p => p.Reviews)
                .ThenInclude(r => r.User)
            .Include(p => p.FavoritedBy)
            .Select(p => new ProductViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                ImageUrl = p.ImageUrl,
                Price = p.Price,
                
                CategoryName = p.Category.Name,
                IsFavorited = userId != null && p.FavoritedBy.Any(f => f.UserId == userId && !f.IsDeleted),
                AverageRating = p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : 0,
                ReviewCount = p.Reviews.Count,
                StockQuantity = p.StockQuantity,
                Reviews = p.Reviews.Select(r => new ReviewViewModel
                {
                    Id = r.Id,
                    UserName = r.User.FullName,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt
                }).ToList()
            })
            .FirstOrDefaultAsync();
    }

    // Get products by category
    public async Task<IEnumerable<ProductViewModel>> GetProductsByCategoryAsync(
        Guid categoryId,
        string? userId)
    {
        return await _uow.Repository<Product, Guid>()
            .Query()
            .Where(p => p.CategoryId == categoryId && p.IsAvailable && !p.IsDeleted)
            .Include(p => p.Category)
            .Include(p => p.Reviews)
            .Include(p => p.FavoritedBy)
            .Select(p => new ProductViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                ImageUrl = p.ImageUrl,
                Price = p.Price,
                CategoryName = p.Category.Name,
                IsFavorited = userId != null && p.FavoritedBy.Any(f => f.UserId == userId && !f.IsDeleted),
                AverageRating = p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : 0,
                ReviewCount = p.Reviews.Count,
                StockQuantity = p.StockQuantity
            })
            .ToListAsync();
    }

    // Get total active products count
    public async Task<int> GetTotalActiveProductsAsync()
    {
        return await _uow.Repository<Product, Guid>()
            .Query()
            .Where(p => p.IsAvailable && !p.IsDeleted)
            .CountAsync();
    }

    // Get all categories for navigation
    public async Task<IEnumerable<CategoryNavViewModel>> GetCategoriesForNavAsync()
    {
        return await _uow.Repository<Category, Guid>()
            .Query()
            .Where(c => !c.IsDeleted)
            .OrderBy(c => c.DisplayOrder)
            .Select(c => new CategoryNavViewModel
            {
                Id = c.Id,
                Name = c.Name,
                IconClass = c.IconClass,
                ProductCount = c.Products.Count(p => p.IsAvailable && !p.IsDeleted)
            })
            .ToListAsync();
    }

    // ============ WRITE OPERATIONS (Client has limited access) ============

    // Add to favorites
    public async Task<bool> AddToFavoritesAsync(string userId, Guid productId)
    {
        var favoriteRepo = _uow.Repository<Favorite, Guid>();

        bool exists = await favoriteRepo
            .Query()
            .AnyAsync(f => f.UserId == userId && f.ProductId == productId && !f.IsDeleted);

        if (!exists)
        {
            await favoriteRepo.AddAsync(new Favorite
            {
                UserId = userId,
                ProductId = productId
            });

            return await _uow.CommitAsync() > 0;
        }

        return false;
    }

    // Remove from favorites
    public async Task<bool> RemoveFromFavoritesAsync(string userId, Guid productId)
    {
        var favoriteRepo = _uow.Repository<Favorite, Guid>();

        var favorite = await favoriteRepo
            .Query()
            .FirstOrDefaultAsync(f => f.UserId == userId && f.ProductId == productId && !f.IsDeleted);

        if (favorite != null)
        {
            await favoriteRepo.DeleteAsync(favorite);
            return await _uow.CommitAsync() > 0;
        }

        return false;
    }

    // Get user's favorite products
    public async Task<IEnumerable<ProductViewModel>> GetUserFavoritesAsync(string userId)
    {
        return await _uow.Repository<Favorite, Guid>()
            .Query()
            .Where(f => f.UserId == userId && !f.IsDeleted)
            .Include(f => f.Product)
                .ThenInclude(p => p.Category)
            .Select(f => new ProductViewModel
            {
                Id = f.Product.Id,
                Name = f.Product.Name,
                Description = f.Product.Description,
                ImageUrl = f.Product.ImageUrl,
                Price = f.Product.Price,
                CategoryName = f.Product.Category.Name,
                IsFavorited = true,
                AverageRating = f.Product.Reviews.Any() ? f.Product.Reviews.Average(r => r.Rating) : 0,
                ReviewCount = f.Product.Reviews.Count
            })
            .ToListAsync();
    }

    // Add a review
    public async Task<bool> AddReviewAsync(string userId, Guid productId, int rating, string? comment)
    {
        // Validate rating range
        if (rating < 1 || rating > 5)
            return false;

        var reviewRepo = _uow.Repository<Review, Guid>();
        var productRepo = _uow.Repository<Product, Guid>();

      
        bool alreadyReviewed = await reviewRepo
            .Query()
            .AnyAsync(r => r.UserId == userId && r.ProductId == productId && !r.IsDeleted);

        if (alreadyReviewed)
            return false;

      
        await _uow.BeginTransactionAsync();

        try
        {
            // Add the review
            var review = new Review
            {
                ProductId = productId,
                UserId = userId,
                Rating = rating,
                Comment = comment
            };

            await reviewRepo.AddAsync(review);

           
            var product = await productRepo.GetByIdAsync(productId);
            if (product != null)
            {
                var allReviews = await reviewRepo
                    .Query()
                    .Where(r => r.ProductId == productId && !r.IsDeleted)
                    .ToListAsync();

                product.AverageRating = allReviews.Average(r => r.Rating);
                await productRepo.UpdateAsync(product);
            }

            await _uow.CommitAsync();
            return true;
        }
        catch
        {
            await _uow.RollbackAsync();
            return false;
        }
    }

    // Check if user has favorited a product
    public async Task<bool> IsFavoritedAsync(string userId, Guid productId)
    {
        return await _uow.Repository<Favorite, Guid>()
            .Query()
            .AnyAsync(f => f.UserId == userId && f.ProductId == productId && !f.IsDeleted);
    }

    // Get user's review for a product
    public async Task<ReviewViewModel?> GetUserReviewAsync(string userId, Guid productId)
    {
        var review = await _uow.Repository<Review, Guid>()
            .Query()
            .FirstOrDefaultAsync(r => r.UserId == userId && r.ProductId == productId && !r.IsDeleted);

        if (review == null)
            return null;

        return new ReviewViewModel
        {
            Id = review.Id,
            Rating = review.Rating,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt
        };
    }
}

