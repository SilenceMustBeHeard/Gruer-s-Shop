using GruersShop.Services.Core.Service.Interfaces.Interactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GruersShop.API.Web.Controllers.Interactions;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ReviewControllerApi : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewControllerApi(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpPost("add/{productId}")]
    public async Task<IActionResult> AddReview(Guid productId, [FromBody] AddReviewRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { error = "You must be logged in to add a review." });
        }

        if (request == null)
        {
            return BadRequest(new { error = "Invalid request." });
        }

        var success = await _reviewService.AddReviewAsync(userId, productId, request.Rating, request.Comment);

        if (!success)
        {
            return Conflict(new { error = "You have already reviewed this product." });
        }

        return Ok(new { success = "Thank you for your review! 🍪" });
    }

    [HttpGet("user-reviews")]
    public async Task<IActionResult> GetUserReviews()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { error = "You must be logged in to view your reviews." });
        }

        var reviews = await _reviewService.GetUserReviewsAsync(userId);

        return Ok(reviews);
    }

    [HttpGet("product/{productId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetProductReviews(Guid productId)
    {
        var reviews = await _reviewService.GetReviewsByProductIdAsync(productId);

        return Ok(reviews);
    }
}

public class AddReviewRequest
{
    public int Rating { get; set; }
    public string? Comment { get; set; }
}