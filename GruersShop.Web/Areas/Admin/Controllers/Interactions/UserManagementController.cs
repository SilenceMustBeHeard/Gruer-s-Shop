using GruersShop.Data.Models.Base;
using GruersShop.Services.Core.Service.Admin.Interfaces.Interactions;
using GruersShop.Web.Areas.Admin.Controllers.Account;
using GruersShop.Web.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GruersShop.Web.Areas.Admin.Controllers.Interactions;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class UserManagementController : BaseAdminController
{
    private readonly IUserManagementService _userService;

    public UserManagementController(
        IUserManagementService userService,
        UserManager<AppUser> userManager) : base(userManager)
    {
        _userService = userService;
    }




    // gets all users with their roles except the  admin
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var allUsers = await _userService
            .GetUserManagmentBoardDataAsync(this.GetUserId());

        return View(allUsers);
    }

    // add role to user

    [HttpPost]
    public async Task<IActionResult> AssignRole(ChangeUserRoleViewModel model)
    {
        if (!ModelState.IsValid
            || string.IsNullOrWhiteSpace(model.NewRole))
        {
            TempData["ErrorMessage"] = "Please select a valid role.";
            return RedirectToAction("Index");
        }

        var result = await _userService.ChangeUserRoleAsync(model, this.GetUserId());

        if (result.Failed)
            TempData["ErrorMessage"] = result.ErrorMessage;
        else
            TempData["SuccessMessage"] = "User role changed successfully.";

        return RedirectToAction("Index");
    }







    // Ban user (soft delete)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DisableUser(string userId)
    {
        var result = await _userService.DisableUserAsync(userId);

        if (result.Failed)
            TempData["ErrorMessage"] = result.ErrorMessage;
        else
            TempData["SuccessMessage"] = "User disabled successfully.";

        return RedirectToAction(nameof(Index));
    }
}