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

    
}

