using GruersShop.Data.Models.Base;
using GruersShop.Services.Core.Service.Admin.Interfaces.Interactions;
using GruersShop.Web.ViewModels.Interactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GruersShop.API.Web.Controllers.Areas.Admin.Interactions;

[Route("api/admin/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class ReviewManagementControllerApi : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IReviewManagementService _reviewManagementService;

    public ReviewManagementControllerApi(
        UserManager<AppUser> userManager,
        IReviewManagementService reviewManagementService)
    {
        _userManager = userManager;
        _reviewManagementService = reviewManagementService;
    }

    [HttpGet("write/{productId}")]
    public async Task<IActionResult> GetWriteReviewModel(Guid productId)
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { error = "You must be logged in to perform this action." });
        }

        var model = await _reviewManagementService.GetWriteReviewModelAsync(userId, productId);

        if (model == null)
        {
            return BadRequest(new { error = "You have already reviewed this design." });
        }

        return Ok(model);
    }

    [HttpPost("post")]
    public async Task<IActionResult> CreateReview([FromBody] AddReviewViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
        }

        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { error = "You must be logged in to perform this action." });
        }

        var result = await _reviewManagementService.CreateReviewAsync(userId, model);

        if (!result.Success)
        {
            return BadRequest(new { error = result.Error ?? "Failed to add review." });
        }

        return Ok(new { success = true, message = "Review added successfully!" });
    }

    [HttpGet("product/{productId}")]
    public async Task<IActionResult> GetProductReviews(Guid productId)
    {
        var reviews = await _reviewManagementService.GetReviewsByProductIdAsync(productId);
        return Ok(reviews);
    }

    [HttpGet("edit-list")]
    public async Task<IActionResult> GetEditList([FromQuery] bool includeDeleted = false)
    {
        IEnumerable<ReviewViewModelList> reviews;

        if (includeDeleted)
        {
            reviews = await _reviewManagementService.GetAllIncludingDeletedAsync();
        }
        else
        {
            reviews = await _reviewManagementService.GetAllActiveAsync();
        }

        return Ok(new
        {
            reviews = reviews.OrderByDescending(r => r.CreatedAt),
            includeDeleted = includeDeleted,
            totalCount = reviews.Count(),
            activeCount = reviews.Count(r => !r.IsDeleted),
            deletedCount = reviews.Count(r => r.IsDeleted)
        });
    }

    [HttpPost("{id}/toggle")]
    public async Task<IActionResult> ToggleReview(Guid id)
    {
        try
        {
            await _reviewManagementService.ToggleReviewAsync(id);

            var review = await _reviewManagementService.GetByIdAsync(id);

            if (review == null)
            {
                return NotFound(new { error = "Review not found." });
            }

            return Ok(new
            {
                success = true,
                message = review.IsDeleted
                    ? "🔒 Review has been hidden from customers!"
                    : "✨ Review is now visible to customers!",
                isDeleted = review.IsDeleted,
                reviewId = id
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Error toggling review status: " + ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetReviewDetails(Guid id)
    {
        var review = await _reviewManagementService.GetByIdAsync(id);

        if (review == null)
        {
            return NotFound(new { error = "Review not found." });
        }

        return Ok(review);
    }

    private string? GetUserId()
    {
        return User?.Identity?.Name;
    }
}