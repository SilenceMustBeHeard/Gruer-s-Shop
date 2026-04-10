using GruersShop.Services.Core.Service.Interfaces.Bakery;
using GruersShop.Web.ViewModels.Bakery;
using GruersShop.Web.ViewModels.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GruersShop.Web.Controllers.Bakery;

public class CatalogController : Controller
{

        private readonly ICatalogClientService _catalogClientService;
            private readonly ICategoryClientService _categoryClientService;
    
    public CatalogController(ICatalogClientService catalogClientService, 
        ICategoryClientService categoryClientService)
    {
        _catalogClientService = catalogClientService;
        _categoryClientService = categoryClientService;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index(int page = 1, int pageSize = 12, Guid? categoryId = null)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isGuest = !User.Identity?.IsAuthenticated ?? true;

        var products = await _catalogClientService.GetPublicCatalogAsync(userId, page, pageSize, isGuest);
        var totalProducts = await _catalogClientService.GetTotalActiveProductsAsync();
        var categories = await _categoryClientService.GetAllActiveCategoriesAsync();

        var viewModel = new CatalogIndexViewModel
        {
            Products = products,
            Categories = categories,
            CurrentPage = page,
            TotalPages = (int)Math.Ceiling(totalProducts / (double)pageSize),
            TotalProducts = totalProducts,
            SelectedCategoryId = categoryId
        };

        return View(viewModel);
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var product = await _catalogClientService.GetProductDetailsAsync(id, userId);

        if (product == null)
            return NotFound();

        var relatedProducts = await _catalogClientService.GetProductsByCategoryAsync(product.CategoryId, userId);
        var userReview = await _catalogClientService.GetUserReviewAsync(userId, id);

        var viewModel = new ProductDetailsViewModel
        {
            Product = product,
            RelatedProducts = relatedProducts.Take(4).ToList(),
            UserHasReviewed = userReview != null,
            UserReview = userReview
        };

        return View(viewModel);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddToFavorites(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _catalogClientService.AddToFavoritesAsync(userId, id);

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddReview(Guid id, int rating, string? comment)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _catalogClientService.AddReviewAsync(userId, id, rating, comment);

        TempData["Success"] = "Thank you for your review!";
        return RedirectToAction(nameof(Details), new { id });
    }
}
