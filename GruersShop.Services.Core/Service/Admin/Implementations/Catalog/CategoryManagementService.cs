using GruersShop.Data.Models.Catalog;
using GruersShop.Data.Repositories.Interfaces.Bakery;
using GruersShop.Services.Core.Service.Admin.Interfaces.Catalog;
using GruersShop.Services.Core.Service.Interfaces.Bakery;
using GruersShop.Web.ViewModels.Admin.Category;
using GruersShop.Web.ViewModels.Bakery;
using GruersShop.Web.ViewModels.Interactions;
using GruersShop.Web.ViewModels.Products;
using System;
using System.Collections.Generic;
using System.Text;

namespace GruersShop.Services.Core.Service.Admin.Implementations.Catalog;

public class CategoryManagementService : ICategoryManagementService
{

    private readonly ICategoryRepository _categoryRepository;


    public CategoryManagementService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task AddCategoryAsync(CategoryViewModelCreate model)
    {
        
        var defaultCatalog = await _categoryRepository.GetDefaultCatalogAsync();

        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = model.Name,
            Description = model.Description,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CatalogId = defaultCatalog.Id, 
            DisplayOrder = 0
        };

        await _categoryRepository.AddAsync(category);
        await _categoryRepository.SaveChangesAsync();
    }

    public async Task EditCategoryAsync(Guid id, CategoryViewModelEdit model)
    {
        var category = await _categoryRepository.GetByIdIncludingDeletedAsync(id);
        if (category == null) return;

        category.Name = model.Name;
        category.Description = model.Description;
        category.IsDeleted = model.IsDeleted;
        category.UpdatedAt = DateTime.UtcNow;
      

        await _categoryRepository.UpdateAsync(category);
        await _categoryRepository.SaveChangesAsync();
    }

    public async Task<IEnumerable<CategoryNavViewModel>> GetAllActiveCategoriesAsync()
    {
        var categories = await _categoryRepository.GetAllActiveAsync();

        return categories.Select(c => new CategoryNavViewModel
        {
            Id = c.Id,
            Name = c.Name,
            IconClass = c.IconClass,
            ProductCount = c.Products.Count
        });
    }

    public async Task<IEnumerable<CategoryViewModelList>> GetAllActiveCategoriesForBulkEditAsync()
    {
        var categories = await _categoryRepository.GetAllActiveAsync();

        return categories.Select(c => new CategoryViewModelList
        {
            Id = c.Id,
            Name = c.Name,
            IsDeleted = c.IsDeleted
        });
    }

    public async Task<IEnumerable<CategoryViewModelList>> GetAllCategoriesForAdminAsync()
    {
        var categories = await _categoryRepository.GetAllForAdminAsync();

        return categories.Select(c => new CategoryViewModelList
        {
            Id = c.Id,
            Name = c.Name,
            IsDeleted = c.IsDeleted
        });
    }

    public async Task<IEnumerable<CategoryNavViewModel>> GetCategoriesWithProductCountAsync()
    {
        var categories = await _categoryRepository.GetAllActiveAsync();

        return categories.Select(c => new CategoryNavViewModel
        {
            Id = c.Id,
            Name = c.Name,
            IconClass = c.IconClass,
            ProductCount = c.Products.Count
        });
    }

    public async Task<CategoryNavViewModel?> GetCategoryByIdAsync(Guid id)
    {

        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null)
            return null;

        return category is Category c ? new CategoryNavViewModel
        {
            Id = c.Id,
            Name = c.Name,
            IconClass = c.IconClass,
            ProductCount = c.Products.Count
        } : null;

    }

    public async Task<CategoryViewModelEdit?> GetCategoryForEditByIdAsync(Guid id)
    {
        var category = await _categoryRepository.GetByIdIncludingDeletedAsync(id);
        if (category == null)
            return null;

        return new CategoryViewModelEdit
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            IsDeleted = category.IsDeleted
        };
    }

    public async Task<CategoryDetailsViewModel?> GetCategoryWithProductsAsync(Guid id, string? userId)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null)
            return null;
        return new CategoryDetailsViewModel
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            IconClass = category.IconClass,
            Products = category.Products.ToList().Select(p => new ProductViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                ImageUrl = p.ImageUrl


            }).ToList(),
            TotalProducts = category.Products.Count

        };
    }

    public async Task ToggleCategoryAsync(Guid id)
    {
        var category = await _categoryRepository.GetByIdIncludingDeletedAsync(id);
        if (category == null) return;

        await _categoryRepository.ToggleCategoryStatusAsync(category);
        await _categoryRepository.SaveChangesAsync();
    }
}
