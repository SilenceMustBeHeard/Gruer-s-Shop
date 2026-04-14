using GruersShop.Services.Core.Service.Interfaces.Bakery;
using GruersShop.Web.ViewModels.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GruersShop.Web.Controllers.Bakery;

[AllowAnonymous]
public class ProductDetailsController : Controller
{
    private readonly ICatalogClientService _catalogClientService;

    public ProductDetailsController(ICatalogClientService catalogClientService)
    {
        _catalogClientService = catalogClientService;
    }

    [HttpGet]
    public virtual async Task<IActionResult> Index(Guid id)
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
}