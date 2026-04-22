using GruersShop.Data.Models.Base;
using GruersShop.Services.Core.Service.Admin.Interfaces.Interactions;
using GruersShop.Services.Core.Service.Interfaces.Interactions;
using GruersShop.Web.Areas.Admin.Controllers.Account;
using GruersShop.Web.ViewModels.Interactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GruersShop.Web.Areas.Admin.Controllers.Interactions;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ReviewManagementController : BaseAdminController
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IReviewManagementService _reviewManagementService;

    public ReviewManagementController(
        UserManager<AppUser> userManager,

        IReviewManagementService reviewManagementService)
        : base(userManager)
    {
        _reviewManagementService = reviewManagementService;
    }

    // gets the form for writing a review for a specific product
    // checks if the user is authorized and if they have already reviewed the product
    // and returns the appropriate view or redirects with an error message
    [HttpGet]
    public async Task<IActionResult> Write(Guid id)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Invalid request.";
            return RedirectToAction("AdminIndex", "CatalogManagement");
        }

        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId.ToString()))
            return Unauthorized();

        var model = await _reviewManagementService.GetWriteReviewModelAsync(userId.ToString(), id);

        if (model == null)
        {
            TempData["Error"] = "You have already reviewed this design.";
            return RedirectToAction("AdminIndex", "CatalogManagement");
        }

        return View(model);
    }

    // posts the review form data to create a new review for a specific product
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Post(AddReviewViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Invalid request.";
            return RedirectToAction("AdminIndex", "CatalogManagement");
        }

        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId.ToString()))
            return Unauthorized();

        var result = await _reviewManagementService.CreateReviewAsync(userId.ToString(), model);

        if (!result.Success)
        {
            TempData["Error"] = result.Error;
            return RedirectToAction("AdminIndex", "CatalogManagement");
        }

        TempData["Success"] = "Review added successfully!";
        return RedirectToAction("AdminIndex", "CatalogManagement");
    }

    // gets the reviews for a specific product and returns the view with the reviews list

    [HttpGet]
    public async Task<IActionResult> Reviews(Guid id)
    {
        var reviews = await _reviewManagementService.GetReviewsByProductIdAsync(id);

        return View(new ReviewListViewModel
        {
            Reviews = reviews.ToList()
        });
    }

    // gets the list of all reviews for editing (including soft deleted ones) and returns the view with the reviews list
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> EditList(bool includeDeleted = false)
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

        ViewBag.IncludeDeleted = includeDeleted;

        return View("EditList", reviews.OrderByDescending(r => r.CreatedAt));
    }

    // toggles the active status of a review (soft delete or restore)
    // by its id and redirects back to the edit list view with a success or error message

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ToggleActive(Guid id)
    {
        try
        {
           
            await _reviewManagementService.ToggleReviewAsync(id);

          
            var review = await _reviewManagementService.GetByIdAsync(id);

            if (review != null)
            {
                TempData["Success"] = review.IsDeleted
                    ? "🔒 Review has been hidden from customers!"
                    : "✨ Review is now visible to customers!";
            }
            else
            {
                TempData["Success"] = "Review status changed successfully!";
            }
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Error toggling review status: " + ex.Message;
        }

        return RedirectToAction(nameof(EditList));
    }

    // get the details for a specific review by its id

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Details(Guid id)
    {
        var review = await _reviewManagementService.GetByIdAsync(id);

        if (review == null)
        {
            TempData["Error"] = "Review not found.";
            return RedirectToAction(nameof(EditList));
        }

        return View(review);
    }
}