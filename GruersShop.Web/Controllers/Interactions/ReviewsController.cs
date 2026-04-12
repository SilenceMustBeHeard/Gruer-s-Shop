using GruersShop.Services.Core.Service.Interfaces.Interactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GruersShop.Web.Controllers.Interactions;

[Authorize]
public class ReviewsController : Controller
{
    private readonly IReviewService _reviewService;

    public ReviewsController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(Guid id, int rating, string? comment)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var success = await _reviewService.AddReviewAsync(userId, id, rating, comment);

        if (!success)
        {
            TempData["Error"] = "You have already reviewed this product.";
            return RedirectToAction("Index", "ProductDetails", new { id });
        }

        TempData["Success"] = "Thank you!";
        return RedirectToAction("Index", "ProductDetails", new { id });
    }

    [HttpGet]
    public async Task<IActionResult> UserReviews()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var reviews = await _reviewService.GetUserReviewsAsync(userId);

        return View(reviews);
    }
}