using GruersShop.Services.Core.Service.Interfaces.Bakery;
using GruersShop.Services.Core.Service.Interfaces.Interactions;
using GruersShop.Web.ViewModels.Bakery;
using GruersShop.Web.ViewModels.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GruersShop.API.Web.Controllers.Areas.User.Bakery;

[Route("api/[controller]")]
[ApiController]
public class CatalogControllerApi : ControllerBase
{
    private readonly ICatalogClientService _catalogService;
    private readonly IFavoriteService _favoriteService;
    private readonly IReviewService _reviewService;

    public CatalogControllerApi(
        ICatalogClientService catalogService,
        IFavoriteService favoriteService,
        IReviewService reviewService)
    {
        _catalogService = catalogService;
        _favoriteService = favoriteService;
        _reviewService = reviewService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<ProductViewModel>>> GetCatalog(
        int page = 1,
        int pageSize = 12,
        Guid? categoryId = null)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isGuest = !User.Identity?.IsAuthenticated ?? true;

        var products = await _catalogService.GetPublicCatalogAsync(
            userId, page, pageSize, isGuest, categoryId);

        return Ok(products);
    }

    [HttpGet("guest")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<ProductViewModel>>> GetGuestCatalog()
    {
        var products = await _catalogService.GetPublicCatalogAsync(null, 1, 3, true, null);
        return Ok(products);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<ProductDetailsViewModel>> GetDetails(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var model = await _catalogService.GetProductDetailsViewModelAsync(id, userId);

        if (model == null)
        {
            return NotFound(new { error = "Product not found" });
        }

        return Ok(model);
    }

    [HttpPost("{id}/favorite")]
    [Authorize]
    public async Task<ActionResult<bool>> ToggleFavorite(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { error = "You must be logged in to favorite products" });
        }

        var isNowFavorited = await _favoriteService.ToggleFavoriteAsync(userId, id);
        return Ok(new { isFavorited = isNowFavorited });
    }

    [HttpPost("{id}/review")]
    [Authorize]
    public async Task<ActionResult> AddReview(Guid id, [FromBody] ReviewRequest request)
    {
        if (request == null)
        {
            return BadRequest(new { error = "Invalid request" });
        }

        if (request.Rating < 1 || request.Rating > 5)
        {
            return BadRequest(new { error = "Rating must be between 1 and 5" });
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { error = "You must be logged in to review products" });
        }

        await _reviewService.AddReviewAsync(userId, id, request.Rating, request.Comment);

        return Ok(new { success = "Review added successfully" });
    }

    [HttpGet("categories")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<CategoryNavViewModel>>> GetCategories()
    {
        var categories = await _catalogService.GetCategoriesForNavAsync();
        return Ok(categories);
    }

    [HttpGet("search")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<ProductViewModel>>> SearchProducts(
        [FromQuery] string? keyword,
        [FromQuery] Guid? categoryId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isGuest = !User.Identity?.IsAuthenticated ?? true;

        var products = await _catalogService.GetPublicCatalogAsync(
            userId, page, pageSize, isGuest, categoryId);

        if (!string.IsNullOrEmpty(keyword))
        {
            products = products.Where(p =>
                p.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                (p.Description?.Contains(keyword, StringComparison.OrdinalIgnoreCase) ?? false));
        }

        return Ok(products);
    }
}

public class ReviewRequest
{
    public int Rating { get; set; }
    public string? Comment { get; set; }
}