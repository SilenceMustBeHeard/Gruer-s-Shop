using GruersShop.Services.Core.Service.Interfaces.Bakery;
using GruersShop.Services.Core.Service.Interfaces.Interactions;
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
    private readonly IFavoriteService _favoriteService;

    public CatalogController(ICatalogClientService catalogClientService,
        ICategoryClientService categoryClientService, IFavoriteService favoriteService)
    {
        _catalogClientService = catalogClientService;
        _categoryClientService = categoryClientService;
        _favoriteService = favoriteService;
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

    // Details
    [HttpGet]
    [AllowAnonymous]
    public virtual async Task<IActionResult> Details(Guid id)
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var model = await _catalogClientService.GetProductDetailsViewModelAsync(id, userId);

        if (model == null)
        {
            TempData["Error"] = "Product not found.";
            return NotFound();
        }

        return View(model);
    }

    // Toggle Favorite
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleFavorite(Guid id, string? returnUrl)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Invalid Action.";
            return RedirectToAction(nameof(Index));
        }

        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        if (userId == null)
        {
            TempData["Error"] = "You must be logged in to manage favorites.";
            return RedirectToAction(nameof(Index));
        }

        bool isNowFavorited = await _favoriteService.ToggleFavoriteAsync(userId, id);

        TempData["Success"] = isNowFavorited
            ? "You added this product to favorites!"
            : "You removed this product from favorites.";

        
        return RedirectToAction(nameof(Index));
    }

    //[HttpPost]
    //[Authorize]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> AddReview(Guid id, int rating, string? comment)
    //{
    //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    //    await _catalogClientService.AddReviewAsync(userId, id, rating, comment);

    //    TempData["Success"] = "Thank you for your review!";
    //    return RedirectToAction(nameof(Details), new { id });
    //}
}