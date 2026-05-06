using GruersShop.Services.Core.Service.Interfaces.Interactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GruersShop.API.Web.Controllers.Interactions;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class FavoriteControllerApi : ControllerBase
{
    private readonly IFavoriteService _favoriteService;

    public FavoriteControllerApi(IFavoriteService favoriteService)
    {
        _favoriteService = favoriteService;
    }

    [HttpPost("toggle/{productId}")]
    public async Task<IActionResult> ToggleFavorite(Guid productId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { error = "You must be logged in to manage favorites." });
        }

        var isNowFavorited = await _favoriteService.ToggleFavoriteAsync(userId, productId);

        return Ok(new
        {
            productId = productId,
            isFavorited = isNowFavorited,
            message = isNowFavorited
                ? "✨ Added to your favorites!"
                : "🗑️ Removed from your favorites."
        });
    }

    [HttpGet("my-favorites")]
    public async Task<IActionResult> GetMyFavorites()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { error = "You must be logged in to view your favorites." });
        }

        var favorites = await _favoriteService.GetUserFavoritesAsync(userId);

        return Ok(favorites);
    }

    [HttpGet("check/{productId}")]
    public async Task<IActionResult> IsFavorite(Guid productId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { error = "You must be logged in to check favorites." });
        }

        var isFavorited = await _favoriteService.IsFavoriteAsync(userId, productId);

        return Ok(new
        {
            productId = productId,
            isFavorited = isFavorited
        });
    }
}