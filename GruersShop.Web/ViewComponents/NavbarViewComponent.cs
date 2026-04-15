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
    private readonly IOrderService _orderService;
    private readonly IContactMessageClientService _contactMessageClientService;
    private readonly IProfileService _profileService;


    public NavbarViewComponent(
            IContactMessageClientService contactMessageClientService,

        SignInManager<AppUser> signInManager,
        UserManager<AppUser> userManager,
        //IOrderService orderService,
        IProfileService profileService)
    {
        _contactMessageClientService = contactMessageClientService;

        _signInManager = signInManager;
        _userManager = userManager;
        //_orderService = orderService;
        _profileService = profileService;
    }



    // It checks if the user is logged in and retrieves their role information to determine which navbar buttons to display.

    // In NavbarViewComponent.cs
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var model = new NavbarButtonsViewModel();

        try
        {
            var userPrincipal = HttpContext.User;

            model.IsLoggedIn = _signInManager.IsSignedIn(userPrincipal);

            if (!model.IsLoggedIn)
                return View(model);

            var user = await _userManager.GetUserAsync(userPrincipal);

            model.IsAdmin = userPrincipal.IsInRole(RoleNames.Admin);
            model.IsManager = userPrincipal.IsInRole(RoleNames.Manager);
            model.IsUser = !model.IsAdmin && !model.IsManager;

            if (user == null)
                return View(model);

            if (model.IsAdmin || model.IsManager)
            {
                // safe defaults
                model.PendingOrdersCount = 0;
                model.UnreadMessagesCount = 0;
            }
            else
            {
                try
                {
                    model.UnreadMessagesCount =
                        await _contactMessageClientService
                            .GetUserUnreadResponsesCountAsync(user.Id);
                }
                catch
                {
                    model.UnreadMessagesCount = 0; // NEVER crash UI
                }
            }
        }
        catch
        {
            // LAST RESORT SAFETY NET
            return View(new NavbarButtonsViewModel
            {
                IsLoggedIn = false
            });
        }

        return View(model);
    }
}
