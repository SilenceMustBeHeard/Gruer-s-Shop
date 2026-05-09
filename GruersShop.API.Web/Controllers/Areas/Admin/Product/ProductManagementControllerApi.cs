using GruersShop.Services.Core.Service.Admin.Interfaces.Catalog;
using GruersShop.Services.Core.Service.Admin.Interfaces.Product;
using GruersShop.Web.ViewModels.Admin.Category;
using GruersShop.Web.ViewModels.Admin.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GruersShop.API.Web.Controllers.Areas.Admin.Product;

[Route("api/admin/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class ProductManagementControllerApi : ControllerBase
{
    private readonly IProductManagementService _productService;
    private readonly ICategoryManagementService _categoryService;

    public ProductManagementControllerApi(
        IProductManagementService productService,
        ICategoryManagementService categoryService)
    {
        _productService = productService;
        _categoryService = categoryService;
    }

    [HttpGet("products")]
    public async Task<IActionResult> GetAllProducts()
    {
        var products = await _productService.GetAllProductsForAdminAsync();

        return Ok(products ?? new List<ProductViewModelList>());
    }

    [HttpPost("{id}/toggle")]
    public async Task<IActionResult> ToggleProduct(Guid id)
    {
        if (id == Guid.Empty)
        {
            return BadRequest(new { error = "Invalid product ID." });
        }

        await _productService.ToggleProductAsync(id);

        return Ok(new
        {
            success = true,
            message = "Product status changed!",
            productId = id
        });
    }

    [HttpGet("create-form")]
    public async Task<IActionResult> GetCreateForm()
    {
        var model = new ProductViewModelCreate();
        await LoadCategoriesAsync(model);

        return Ok(new
        {
            model = model,
            categories = model.Categories
        });
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateProduct([FromBody] ProductViewModelCreate model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
            });
        }

        await _productService.AddProductAsync(model);

        return Ok(new
        {
            success = true,
            message = "Product created successfully! 🍪",
        });
    }

    [HttpGet("{id}/edit")]
    public async Task<IActionResult> GetProductForEdit(Guid id)
    {
        var model = await _productService.GetProductForEditByIdAsync(id);

        if (model == null)
        {
            return NotFound(new { error = "Product not found." });
        }

        await LoadCategoriesAsync(model);

        return Ok(new
        {
            product = model,
            categories = model.Categories
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] ProductViewModelEdit model)
    {
        if (id != model.Id)
        {
            return BadRequest(new { error = "ID mismatch" });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
            });
        }

        await _productService.EditProductAsync(model.Id, model);

        return Ok(new
        {
            success = true,
            message = "Product updated successfully! 🥐",
            productId = model.Id
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductDetails(Guid id)
    {
        if (id == Guid.Empty)
        {
            return BadRequest(new { error = "Invalid product ID." });
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { error = "User not authenticated." });
        }

        var model = await _productService.GetProductDetailsViewModelAsync(id, userId);

        if (model == null)
        {
            return NotFound(new { error = "Product not found." });
        }

        return Ok(model);
    }

    private async Task LoadCategoriesAsync(ProductViewModelCreate model)
    {
        var categories = await _categoryService.GetAllCategoriesForAdminAsync();
        model.Categories = categories.Select(c => new CategorySelectViewModel
        {
            Id = c.Id,
            Name = c.Name
        });
    }

    private async Task LoadCategoriesAsync(ProductViewModelEdit model)
    {
        var categories = await _categoryService.GetAllCategoriesForAdminAsync();
        model.Categories = categories.Select(c => new CategorySelectViewModel
        {
            Id = c.Id,
            Name = c.Name
        });
    }
}