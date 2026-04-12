using GruersShop.Services.Core.Service.Interfaces.Bakery;
using GruersShop.Web.ViewModels.Bakery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GruersShop.Web.Controllers.Bakery;

[AllowAnonymous]
[Route("bakery/catalog")]
public class CatalogController : Controller
{
    private readonly ICatalogClientService _catalogClientService;
    private readonly ICategoryClientService _categoryClientService;

    public CatalogController(
        ICatalogClientService catalogClientService,
        ICategoryClientService categoryClientService)
    {
        _catalogClientService = catalogClientService;
        _categoryClientService = categoryClientService;
    }

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
}