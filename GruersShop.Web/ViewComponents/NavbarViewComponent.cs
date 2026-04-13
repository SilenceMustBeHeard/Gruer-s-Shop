using GruersShop.Data.Models.Base;
using GruersShop.Services.Core.Service.Interfaces.Interactions;
using GruersShop.Web.ViewModels.Account.Profile;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GruersShop.Web.ViewComponents;

public class NavbarViewComponent : ViewComponent
{
    private readonly SignInManager<AppUser> _signInManager;
    private readonly UserManager<AppUser> _userManager;
    private readonly IFavoriteService _favoriteService;
    private readonly IReviewService _reviewService;

    public NavbarViewComponent(
        SignInManager<AppUser> signInManager,
        UserManager<AppUser> userManager,
        IFavoriteService favoriteService,
        IReviewService reviewService)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _favoriteService = favoriteService;
        _reviewService = reviewService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var model = new NavbarButtonsViewModel
        {
            IsLoggedIn = _signInManager.IsSignedIn(HttpContext.User)
        };

        if (model.IsLoggedIn)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var roles = await _userManager.GetRolesAsync(user);

            model.IsAdmin = roles.Contains("Admin");
            model.IsManager = roles.Contains("Manager");
            model.IsUser = !model.IsAdmin && !model.IsManager;

            if (user != null && model.IsUser)
            {
                // Get unread counts for user
                var favorites = await _favoriteService.GetUserFavoritesAsync(user.Id);
                var reviews = await _reviewService.GetUserReviewsAsync(user.Id);
                model.UnreadMessagesCount = 0; // You can implement message count later
            }
        }

        return View(model);
    }
}