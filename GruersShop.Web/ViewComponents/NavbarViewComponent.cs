using GruersShop.Data.Models.Base;
using GruersShop.Services.Common;
using GruersShop.Services.Core.Service.Interfaces.Account;
using GruersShop.Services.Core.Service.Interfaces.Messages;
using GruersShop.Web.ViewModels.Account.Profile;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GruersShop.Web.ViewComponents;

public class NavbarViewComponent : Microsoft.AspNetCore.Mvc.ViewComponent
{
    private readonly SignInManager<AppUser> _signInManager;
 
    private readonly IContactMessageClientService _contactMessageClientService;
    private readonly IProfileService _profileService;
    private readonly UserManager<AppUser> _userManager;


    public NavbarViewComponent(
            IContactMessageClientService contactMessageClientService,
         
        SignInManager<AppUser> signInManager,
        UserManager<AppUser> userManager,
      
    IProfileService profileService)
    {
        _contactMessageClientService = contactMessageClientService;
      
        _signInManager = signInManager;
        _userManager = userManager;
     
        _profileService = profileService;
    }



    // It checks if the user is logged in and retrieves their role information to determine which navbar buttons to display.

    // In NavbarViewComponent.cs
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var model = new NavbarButtonsViewModel
        {
          
        };

        if (model.IsLoggedIn)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            model.IsAdmin = HttpContext.User.IsInRole(RoleNames.Admin);
            model.IsManager = HttpContext.User.IsInRole(RoleNames.Manager);
            model.IsUser = !model.IsAdmin && !model.IsManager;

            if (user != null)
            {
                if (model.IsAdmin || model.IsManager)
                {

                   
                }
                else
                {

                 
                    var contactResponseUnread = await _contactMessageClientService.GetUserUnreadResponsesCountAsync(user.Id);
                    model.UnreadMessagesCount =  contactResponseUnread;
                }
            }
        }

        return View(model);
    }
}