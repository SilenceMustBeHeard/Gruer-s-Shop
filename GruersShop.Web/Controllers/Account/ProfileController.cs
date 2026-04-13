using GruersShop.Data.Models.Base;
using GruersShop.Services.Core.Service.Admin.Interfaces.Message;
using GruersShop.Services.Core.Service.Interfaces.Account;
using GruersShop.Services.Core.Service.Interfaces.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GruersShop.Web.Controllers.Account;

[Authorize]

public class ProfileController : Controller
{
    private readonly IProfileService _profileService;
    private readonly UserManager<AppUser> _userManager;

    private readonly ISystemInboxMessageService _systemInboxMessageService;
    private readonly IContactMessageClientService _contactMessageClientService;

    public ProfileController(
        IContactMessageClientService contactMessageClientService,
        ISystemInboxMessageService systemInboxMessageService,
        IProfileService profileService,
        UserManager<AppUser> userManager
        )
        


    {
        _contactMessageClientService = contactMessageClientService;
        _systemInboxMessageService = systemInboxMessageService;
        _profileService = profileService;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            TempData["Error"] = "You must be logged in to perform this action.";
            return RedirectToAction("Login", "Account");

        }

        var model = await _profileService.GetProfileAsync(user.Id);
        return View(model);
    }

  

   
    



    [HttpGet]
    public async Task<IActionResult> SystemMessageDetails(Guid id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            TempData["Error"] = "You must be logged in to perform this action.";
            return RedirectToAction("Login", "Account");
        }

        var viewModel = await _systemInboxMessageService.GetMessageDetailsAsync(id, user.Id);

        if (viewModel == null)
        {
            TempData["Error"] = "Message not found or you do not have permission to view it.";
            return NotFound();
        }

        return View("SystemMessageDetails", viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> ContactMessageDetails(Guid id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            TempData["Error"] = "You must be logged in to perform this action.";
            return RedirectToAction("Login", "Account");
        }


        var viewModel = await _contactMessageClientService.GetMessageDetailsAsync(id, user.Id);

        if (viewModel == null)
        {
            TempData["Error"] = "Message not found or you do not have permission to view it.";
            return NotFound();
        }

        return View(viewModel);
    }

   
}