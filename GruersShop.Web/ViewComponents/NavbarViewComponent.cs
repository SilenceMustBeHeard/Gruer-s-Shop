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
        var model = new NavbarButtonsViewModel
        {
            IsLoggedIn = _signInManager.IsSignedIn(HttpContext.User)
        };

        if (model.IsLoggedIn)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            model.IsAdmin = HttpContext.User.IsInRole(RoleNames.Admin);
            model.IsManager = HttpContext.User.IsInRole(RoleNames.Manager);
            model.IsUser = !model.IsAdmin && !model.IsManager;

            if (user != null && model.IsUser)
            {
                try
                {
                    model.UnreadMessagesCount = await _contactMessageClientService
                        .GetUserUnreadResponsesCountAsync(user.Id);
                }
                catch
                {
                    model.UnreadMessagesCount = 0;
                }
            }
        }

        return View(model);
    }
}