using GruersShop.Data.Models.Products;
using GruersShop.Data.Repositories.Interfaces.Bakery;
using GruersShop.Services.Core.Service.Admin.Interfaces.Catalog;
using GruersShop.Services.Core.Service.Admin.Interfaces.Product;
using GruersShop.Web.ViewModels.Admin.Products;
using GruersShop.Web.ViewModels.Bakery;
using GruersShop.Web.ViewModels.Products;

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





    public async Task AddProductAsync(ProductViewModelCreate model)
    {
        var product = new Product
        {
            Name = model.Name,
            Description = model.Description,
            Price = model.Price,
            ImageUrl = model.ImageUrl,
            IsAvailable = model.IsAvailable,
            CategoryId = model.CategoryId,
            StockQuantity = model.StockQuantity
          
        };
        
         await _productRepository.AddAsync(product);  
    }
    

    public async Task EditProductAsync(Guid id, ProductViewModelEdit model)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<ProductViewModelList>> GetAllActiveProductsAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<ProductViewModelList>> GetAllProductsForAdminAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<CategoryNavViewModel>> GetCategoriesForNavAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<ProductViewModel?> GetProductDetailsAsync(Guid id, string? userId)
    {
        throw new NotImplementedException();
    }

    public async Task<ProductDetailsViewModel?> GetProductDetailsViewModelAsync(Guid id, string? userId)
    {
        throw new NotImplementedException();
    }

    public async Task<ProductViewModelEdit?> GetProductForEditByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<ProductViewModel>> GetProductsByCategoryAsync(Guid categoryId, string? userId)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<ProductViewModel>> GetPublicCatalogAsync(string? userId, int page, int pageSize, bool isGuest)
    {
        throw new NotImplementedException();
    }

    public async Task<int> GetTotalActiveProductsAsync()
    {
        throw new NotImplementedException();
    }

    public async Task ToggleProductAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}
