using GruersShop.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GruersShop.Web.Controllers.Account;

[Authorize]
public abstract class BaseController : Controller
{
    protected BaseController(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }
    // Helper methods for user information and role checks
    protected bool IsUserAdmin() => User.IsInRole("Admin");

    // Checks if the user is authenticated
    protected bool IsUserAuthenticated() => User.Identity?.IsAuthenticated ?? false;



    private readonly UserManager<AppUser> _userManager;

    // Retrieves the current user's ID, or null if not authenticated
    protected string? GetUserId()
    {
        if (User?.Identity == null || !User.Identity.IsAuthenticated)
            return null;

        return _userManager.GetUserId(User);
    }

    // Retrieves the current user
    protected async Task<AppUser?> GetCurrentUserAsync()
    {
        return await _userManager.GetUserAsync(User);
    }

}
