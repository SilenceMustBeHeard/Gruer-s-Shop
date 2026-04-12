using GruersShop.Services.Core.Service.Interfaces.Interactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GruersShop.Web.Controllers.Interactions;

[Authorize]
public class FavoritesController : Controller
{
    private readonly IFavoriteService _favoriteService;

    public FavoritesController(IFavoriteService favoriteService)
    {
        _favoriteService = favoriteService;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Toggle(Guid id, string? returnUrl)
    {
        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        if (string.IsNullOrEmpty(userId))
        {
            TempData["Error"] = "You must be logged in to manage favorites.";
            return RedirectToAction("Index", "Catalog");
        }

        bool isNowFavorited = await _favoriteService.ToggleFavoriteAsync(userId, id);

        TempData["Success"] = isNowFavorited
            ? "✨ Added to your favorites!"
            : "🗑️ Removed from your favorites.";

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "ProductDetails", new { id });
    }

    public async Task<IActionResult> MyFavorites()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var favorites = await _favoriteService.GetUserFavoritesAsync(userId);

        return View(favorites);
    }
}