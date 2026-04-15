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
            IsLoggedIn = _signInManager.IsSignedIn(HttpContext.User),
            CurrentArea = RouteData.Values["area"]?.ToString()
        };
        if (model.IsLoggedIn)
        {
            model.IsAdmin = HttpContext.User.IsInRole("Admin");
            model.IsManager = HttpContext.User.IsInRole("Manager");
            model.IsUser = !model.IsAdmin && !model.IsManager;

            if (model.IsAdmin || model.IsManager)
            {
                model.PendingOrdersCount = 0; // TODO
                model.UnreadMessagesCount = 0; // TODO
            }
            else
            {
                model.UnreadMessagesCount = 0;
            }
        }

        return View(model);
    }
}