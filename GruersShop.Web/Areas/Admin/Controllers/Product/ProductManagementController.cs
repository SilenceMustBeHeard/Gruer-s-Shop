using GruersShop.Services.Core.Service.Admin.Interfaces.Catalog;
using GruersShop.Services.Core.Service.Admin.Interfaces.Product;
using GruersShop.Services.Core.Service.Interfaces.Bakery;
using GruersShop.Web.ViewModels.Admin.Category;
using GruersShop.Web.ViewModels.Admin.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace GruersShop.Web.Areas.Admin.Controllers.Product;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ProductManagementController : Controller
{
    private readonly IProductManagementService _productService;
    private readonly ICategoryManagementService _categoryService;

    public ProductManagementController(
        IProductManagementService productService,
        ICategoryManagementService categoryService)
    {
        _productService = productService;
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<IActionResult> EditList()
    {
        var products = await _productService.GetAllProductsForAdminAsync();
        return View("EditList", products);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id)
    {
        await _productService.ToggleProductAsync(id);
        TempData["Success"] = "Product status changed!";
        return RedirectToAction(nameof(EditList));
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var model = new ProductViewModelCreate();
        await LoadCategoriesAsync(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductViewModelCreate model)
    {
        if (!ModelState.IsValid)
        {
            await LoadCategoriesAsync(model);
            return View(model);
        }

        await _productService.AddProductAsync(model);
        TempData["Success"] = "Product created successfully! 🍪";
        return RedirectToAction(nameof(EditList));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var model = await _productService.GetProductForEditByIdAsync(id);
        if (model == null) return NotFound();

        await LoadCategoriesAsync(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ProductViewModelEdit model)
    {
        if (!ModelState.IsValid)
        {
            await LoadCategoriesAsync(model);
            return View(model);
        }

        await _productService.EditProductAsync(model.Id, model);
        TempData["Success"] = "Product updated successfully! 🥐";
        return RedirectToAction(nameof(EditList));
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var model = await _productService.GetProductDetailsViewModelAsync(id, userId);

        if (model == null)
        {
            TempData["Error"] = "Product not found.";
            return NotFound();
        }

        return View(model);
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