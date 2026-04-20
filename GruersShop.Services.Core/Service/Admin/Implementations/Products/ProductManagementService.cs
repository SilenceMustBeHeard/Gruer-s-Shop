using GruersShop.Data.Models.Products;
using GruersShop.Data.Repositories.Interfaces.Bakery;
using GruersShop.Services.Core.Service.Admin.Interfaces.Catalog;
using GruersShop.Services.Core.Service.Admin.Interfaces.Product;
using GruersShop.Web.ViewModels.Admin.Category;
using GruersShop.Web.ViewModels.Admin.Products;
using GruersShop.Web.ViewModels.Bakery;
using GruersShop.Web.ViewModels.Interactions;
using GruersShop.Web.ViewModels.Products;
using Microsoft.EntityFrameworkCore;

namespace GruersShop.Services.Core.Service.Admin.Implementations.Products;

public class ProductManagementService : IProductManagementService
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryManagementService _categoryService;

    public ProductManagementService(ICategoryManagementService categoryService,
        IProductRepository productRepository)
    {
        _productRepository = productRepository;
        _categoryService = categoryService;
    }




    // adds a new product to the database based on the provided view model,
    // setting all necessary properties and saving changes asynchronously
    public async Task AddProductAsync(ProductViewModelCreate model)
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = model.Name,
            Description = model.Description,
            Price = model.Price,
            ImageUrl = model.ImageUrl,
            IsAvailable = model.IsAvailable,
            CategoryId = model.CategoryId,
            StockQuantity = model.StockQuantity,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        await _productRepository.AddAsync(product);
        await _productRepository.SaveChangesAsync();
    }


    // updates an existing product in the database based on the provided view model,
    public async Task EditProductAsync(Guid id, ProductViewModelEdit model)
    {
        var product = await _productRepository.GetByIdIncludingDeletedAsync(id);
        if (product == null) return;

        product.Name = model.Name;
        product.Description = model.Description;
        product.Price = model.Price;
        product.ImageUrl = model.ImageUrl;
        product.IsAvailable = model.IsAvailable;
        product.CategoryId = model.CategoryId;
        product.StockQuantity = model.StockQuantity;
        product.UpdatedAt = DateTime.UtcNow;

        await _productRepository.UpdateAsync(product);
        await _productRepository.SaveChangesAsync();
    }


    // retrieves all active products from the database, maps them to a list of view models,
    public async Task<IEnumerable<ProductViewModelList>> GetAllActiveProductsAsync()
    {
        var products = await _productRepository.GetAllActiveAsync();

        return products.Select(p => new ProductViewModelList
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            ImageUrl = p.ImageUrl,
            IsAvailable = p.IsAvailable,
            StockQuantity = p.StockQuantity,
            CategoryName = p.Category?.Name ?? "Uncategorized",
            CreatedOn = p.CreatedAt,
            IsDeleted = p.IsDeleted
        });
    }


    // retrieves all products (including deleted) from the database, maps them to a list of view models,
    public async Task<IEnumerable<ProductViewModelList>> GetAllProductsForAdminAsync()
    {
        var products = await _productRepository.GetAllForAdminAsync();

        return products.Select(p => new ProductViewModelList
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            ImageUrl = p.ImageUrl,
            IsAvailable = p.IsAvailable,
            StockQuantity = p.StockQuantity,
            CategoryName = p.Category?.Name ?? "Uncategorized",
            CreatedOn = p.CreatedAt,
            IsDeleted = p.IsDeleted
        });
    }

    // retrieves all active categories from the database, maps them to a list of view models suitable for navigation display
    public async Task<IEnumerable<CategoryNavViewModel>> GetCategoriesForNavAsync()
    {
        return await _categoryService.GetAllActiveCategoriesAsync();
    }
    // retrieves detailed information about a specific product by its ID,
    // including related reviews and category information,
    public async Task<ProductViewModel?> GetProductDetailsAsync(Guid id, string? userId)
    {
        var product = await _productRepository.GetByIdWithDetailsAsync(id);
        if (product == null) return null;

        return new ProductViewModel
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            ImageUrl = product.ImageUrl,
            CategoryId = product.CategoryId,
            CategoryName = product.Category?.Name ?? "Uncategorized",
            AverageRating = product.AverageRating,
            StockQuantity = product.StockQuantity,
            IsFavorited = false,
            ReviewCount = product.Reviews.Count,
            Reviews = product.Reviews.Select(r => new ReviewViewModel
            {
                Id = r.Id,
                Rating = r.Rating,
                Comment = r.Comment,
                UserName = r.User?.FullName ?? "Anonymous",
                CreatedAt = r.CreatedAt
            }).ToList()
        };
    }


    // retrieves detailed information about a specific product by its ID, including related reviews, category information,
    public async Task<ProductDetailsViewModel?> GetProductDetailsViewModelAsync(Guid id, string? userId)
    {
        var product = await _productRepository.GetByIdWithDetailsAsync(id);
        if (product == null) return null;

        var productViewModel = new ProductViewModel
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            ImageUrl = product.ImageUrl,
            CategoryId = product.CategoryId,
            CategoryName = product.Category?.Name ?? "Uncategorized",
            AverageRating = product.AverageRating,
            StockQuantity = product.StockQuantity,
            IsFavorited = false,
            ReviewCount = product.Reviews.Count,
            Reviews = product.Reviews.Select(r => new ReviewViewModel
            {
                Id = r.Id,
                Rating = r.Rating,
                Comment = r.Comment,
                UserName = r.User?.FullName ?? "Anonymous",
                CreatedAt = r.CreatedAt
            }).ToList()
        };

        // Check if user has reviewed
        bool userHasReviewed = false;
        ReviewViewModel? userReview = null;

        if (!string.IsNullOrEmpty(userId))
        {
            userHasReviewed = await _productRepository.HasUserReviewedAsync(userId, id);
            if (userHasReviewed)
            {
                var review = product.Reviews.FirstOrDefault(r => r.UserId == userId);
                if (review != null)
                {
                    userReview = new ReviewViewModel
                    {
                        Id = review.Id,
                        Rating = review.Rating,
                        Comment = review.Comment,
                        UserName = review.User?.FullName ?? "Anonymous",
                        CreatedAt = review.CreatedAt
                    };
                }
            }
        }

        // Get related products from same category
        var relatedProducts = await _productRepository.GetByCategoryIdAsync(product.CategoryId);
        var relatedProductViewModels = relatedProducts
            .Where(p => p.Id != id)
            .Take(4)
            .Select(p => new ProductViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                ImageUrl = p.ImageUrl,
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.Name ?? "Uncategorized",
                AverageRating = p.AverageRating,
                StockQuantity = p.StockQuantity,
                IsFavorited = false,
                ReviewCount = p.Reviews.Count
            }).ToList();

        return new ProductDetailsViewModel
        {
            Product = productViewModel,
            UserHasReviewed = userHasReviewed,
            UserReview = userReview,
            NewRating = 5,
            NewComment = null,
            RelatedProducts = relatedProductViewModels
        };
    }

    public async Task<ProductViewModelEdit?> GetProductForEditByIdAsync(Guid id)
    {
        var product = await _productRepository.GetByIdIncludingDeletedAsync(id);
        if (product == null) return null;

        // Get categories for dropdown
        var categories = await _categoryService.GetAllCategoriesForAdminAsync();

        return new ProductViewModelEdit
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            ImageUrl = product.ImageUrl,
            IsAvailable = product.IsAvailable,
            CategoryId = product.CategoryId,
            StockQuantity = product.StockQuantity,
            CurrentImageUrl = product.ImageUrl,
            CategoryName = product.Category?.Name ?? "Uncategorized",
            Categories = categories.Select(c => new CategorySelectViewModel
            {
                Id = c.Id,
                Name = c.Name
            })
        };
    }

    public async Task<IEnumerable<ProductViewModel>> GetProductsByCategoryAsync(Guid categoryId, string? userId)
    {
        var products = await _productRepository.GetByCategoryIdAsync(categoryId);

        return products.Select(p => new ProductViewModel
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            ImageUrl = p.ImageUrl,
            CategoryId = p.CategoryId,
            CategoryName = p.Category?.Name ?? "Uncategorized",
            AverageRating = p.AverageRating,
            StockQuantity = p.StockQuantity,
            IsFavorited = false,
            ReviewCount = p.Reviews.Count
        });
    }

    public async Task<IEnumerable<ProductViewModel>> GetPublicCatalogAsync(string? userId, int page, int pageSize, bool isGuest)
    {
        var products = await _productRepository.GetPagedActiveProductsAsync(page, pageSize);

        return products.Select(p => new ProductViewModel
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            ImageUrl = p.ImageUrl,
            CategoryId = p.CategoryId,
            CategoryName = p.Category?.Name ?? "Uncategorized",
            AverageRating = p.AverageRating,
            StockQuantity = p.StockQuantity,
            IsFavorited = false,
            ReviewCount = p.Reviews.Count
        });
    }

    public async Task<int> GetTotalActiveProductsAsync()
    {
        return await _productRepository.CountActiveAsync();
    }

    public async Task ToggleProductAsync(Guid id)
    {
        var product = await _productRepository.GetByIdIncludingDeletedAsync(id);
        if (product == null) return;

        product.IsAvailable = !product.IsAvailable;
        product.UpdatedAt = DateTime.UtcNow;

        await _productRepository.UpdateAsync(product);
        await _productRepository.SaveChangesAsync();
    }
}