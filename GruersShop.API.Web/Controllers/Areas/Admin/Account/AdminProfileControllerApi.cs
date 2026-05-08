using GruersShop.Data.Models.Base;
using GruersShop.Services.Core.Service.Admin.Interfaces.Message;
using GruersShop.Services.Core.Service.Interfaces.Account;
using GruersShop.Web.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SendGrid.Helpers.Errors.Model;

namespace GruersShop.API.Web.Controllers.Areas.Admin.Account;

[Route("api/admin/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class AdminProfileControllerApi : ControllerBase
{
    private readonly IProfileService _profileService;
    private readonly UserManager<AppUser> _userManager;
    private readonly ISystemInboxMessageService _systemInboxMessageService;
    private readonly IContactMessageService _contactMessageService;

    public AdminProfileControllerApi(
        ISystemInboxMessageService systemInboxMessageService,
        IProfileService profileService,
        UserManager<AppUser> userManager,
        IContactMessageService contactMessageService)
    {
        _systemInboxMessageService = systemInboxMessageService;
        _profileService = profileService;
        _userManager = userManager;
        _contactMessageService = contactMessageService;
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized(new { error = "You must be logged in to perform this action." });
        }

        var model = await _profileService.GetProfileAsync(user.Id);
        if (model == null)
        {
            return StatusCode(500, new { error = "Unable to load profile. Please try again later." });
        }

        return Ok(model);
    }

    [HttpGet("system-inbox")]
    public async Task<IActionResult> GetSystemInbox()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized(new { error = "You must be logged in to perform this action." });
        }

        var messages = await _systemInboxMessageService.GetAdminMessagesAsync(user.Id);

        if (messages == null)
        {
            return StatusCode(500, new { error = "Unable to load system inbox messages." });
        }

        return Ok(messages);
    }

    [HttpGet("system-message/{id}")]
    public async Task<IActionResult> GetSystemMessageDetails(Guid id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized(new { error = "You must be logged in to perform this action." });
        }

        var viewModel = await _systemInboxMessageService.GetMessageDetailsAsync(id, user.Id);

        if (viewModel == null)
        {
            return NotFound(new { error = "Message not found or you do not have permission to view it." });
        }

        return Ok(viewModel);
    }

    [HttpGet("contact-inbox")]
    public async Task<IActionResult> GetContactInbox()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized(new { error = "You must be logged in to perform this action." });
        }

        var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
        var isManager = await _userManager.IsInRoleAsync(user, "Manager");

        if (!isAdmin && !isManager)
        {
            return Unauthorized(new { error = "You don't have permission to access this page." });
        }

        var userId = _userManager.GetUserId(User);
        if (userId == null)
        {
            return Unauthorized(new { error = "You must be logged in to perform this action." });
        }

        var contactMessages = await _contactMessageService.GetAdminMessagesAsync(userId);

        var model = new AdminInboxViewModel
        {
            ContactMessages = contactMessages
        };

        return Ok(model);
    }
}