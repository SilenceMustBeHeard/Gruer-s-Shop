using GruersShop.Data.Models.Catalog;
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


public class CategoryClientService : ICategoryClientService
{
    private readonly IUnitOfWork _uow;

    public CategoryClientService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<IEnumerable<CategoryNavViewModel>> GetAllActiveCategoriesAsync()
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

    public async Task<CategoryNavViewModel?> GetCategoryByIdAsync(Guid id)
    {
        var category = await _uow.Repository<Category, Guid>()
            .Query()
            .Where(c => c.Id == id && !c.IsDeleted)
            .Select(c => new CategoryNavViewModel
            {
                Id = c.Id,
                Name = c.Name,
                IconClass = c.IconClass,
                ProductCount = c.Products.Count(p => p.IsAvailable && !p.IsDeleted)
            })
            .FirstOrDefaultAsync();

        return category;
    }

    public async Task<IEnumerable<CategoryNavViewModel>> GetCategoriesWithProductCountAsync()
    {
        return await _uow.Repository<Category, Guid>()
            .Query()
            .Where(c => !c.IsDeleted && c.Products.Any(p => p.IsAvailable && !p.IsDeleted))
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

    public async Task<CategoryDetailsViewModel?> GetCategoryWithProductsAsync(Guid id, string? userId)
    {
        var category = await _uow.Repository<Category, Guid>()
            .Query()
            .Where(c => c.Id == id && !c.IsDeleted)
            .Include(c => c.Products.Where(p => p.IsAvailable && !p.IsDeleted))
                .ThenInclude(p => p.Reviews)
            .Include(c => c.Products)
                .ThenInclude(p => p.FavoritedBy)
            .FirstOrDefaultAsync();

        if (category == null)
            return null;

        return new CategoryDetailsViewModel
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            IconClass = category.IconClass,
            Products = category.Products.Select(p => new ProductViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                ImageUrl = p.ImageUrl,
                Price = p.Price,
                CategoryName = category.Name,
                IsFavorited = userId != null && p.FavoritedBy.Any(f => f.UserId == userId && !f.IsDeleted),
                AverageRating = p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : 0,
                ReviewCount = p.Reviews.Count,
                StockQuantity = p.StockQuantity
            }).ToList(),
            TotalProducts = category.Products.Count(p => p.IsAvailable && !p.IsDeleted)
        };
    }
}