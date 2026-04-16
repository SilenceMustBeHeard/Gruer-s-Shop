using GruersShop.Data.Models.Base;
using GruersShop.Services.Common;
using GruersShop.Services.Core.Service.Interfaces.Account;
using GruersShop.Services.Core.Service.Interfaces.Interactions;
using GruersShop.Services.Core.Service.Interfaces.Messages;
using GruersShop.Web.ViewModels.Account.Profile;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GruersShop.Web.ViewComponents;

public class NavbarViewComponent : ViewComponent
{
    private readonly SignInManager<AppUser> _signInManager;
    private readonly UserManager<AppUser> _userManager;
    private readonly IContactMessageClientService _contactMessageClientService;

    public NavbarViewComponent(
        SignInManager<AppUser> signInManager,
        UserManager<AppUser> userManager,
        IContactMessageClientService contactMessageClientService)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _contactMessageClientService = contactMessageClientService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var isLoggedIn = _signInManager.IsSignedIn(HttpContext.User);

        var model = new NavbarButtonsViewModel
        {
            IsLoggedIn = isLoggedIn
        };

        if (!isLoggedIn)
        {
            model.IsAdmin = false;
            model.IsManager = false;
            model.IsUser = false;
            model.UnreadMessagesCount = 0;

            return View(model);
        }

        var user = await _userManager.GetUserAsync(HttpContext.User);

        if (user == null)
        {
            // 🔥 CRITICAL SAFETY NET (logout race condition)
            return View(model);
        }

        model.IsAdmin = HttpContext.User.IsInRole(RoleNames.Admin);
        model.IsManager = HttpContext.User.IsInRole(RoleNames.Manager);
        model.IsUser = !model.IsAdmin && !model.IsManager;

        if (model.IsUser)
        {
            try
            {
                model.UnreadMessagesCount =
                    await _contactMessageClientService.GetUserUnreadResponsesCountAsync(user.Id);
            }
            catch
            {
                model.UnreadMessagesCount = 0;
            }
        }

        return View(model);
    }
}