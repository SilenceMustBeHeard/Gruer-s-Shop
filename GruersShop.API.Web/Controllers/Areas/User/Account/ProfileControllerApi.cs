using GruersShop.Data.Models.Base;
using GruersShop.Services.Core.Service.Interfaces.Account;
using GruersShop.Services.Core.Service.Interfaces.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GruersShop.API.Web.Controllers.Areas.User.Account;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ProfileControllerApi : ControllerBase
{
    private readonly IProfileService _profileService;
    private readonly UserManager<AppUser> _userManager;
    private readonly ISystemInboxClientService _systemInboxClientService;
    private readonly IContactMessageClientService _contactMessageClientService;

    public ProfileControllerApi(
        IContactMessageClientService contactMessageClientService,
        ISystemInboxClientService systemInboxClientService,
        IProfileService profileService,
        UserManager<AppUser> userManager)
    {
        _contactMessageClientService = contactMessageClientService;
        _systemInboxClientService = systemInboxClientService;
        _profileService = profileService;
        _userManager = userManager;
    }

    [HttpGet("index")]
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return BadRequest(new { error = "You must be logged in to access this resource." });
        }

        var model = await _profileService.GetProfileAsync(user.Id);
        return Ok(model);
    }

    [HttpGet("system-message-details")]
    public async Task<IActionResult> SystemMessageDetails(Guid id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return BadRequest(new { error = "You must be logged in to perform this action." });
        }

        var viewModel = await _systemInboxClientService.GetMessageDetailsAsync(id, user.Id);

        if (viewModel == null)
        {
            return NotFound(new { error = "Message not found or you do not have permission to view it." });
        }

        return Ok(viewModel);
    }

    [HttpGet("contact-message-details")]
    public async Task<IActionResult> ContactMessageDetails(Guid id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return BadRequest(new { error = "You must be logged in to perform this action." });
        }

        // Маркирай като прочетено
        await _contactMessageClientService.MarkAsReadAsync(id, user.Id);

        var viewModel = await _contactMessageClientService.GetMessageDetailsAsync(id, user.Id);

        if (viewModel == null)
        {
            return NotFound(new { error = "Message not found or you do not have permission to view it." });
        }

        return Ok(viewModel);
    }

    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return BadRequest(new { error = "You must be logged in to perform this action." });
        }

        var unreadCount = await _contactMessageClientService.GetUserUnreadResponsesCountAsync(user.Id);
        return Ok(new { unreadCount });
    }

    [HttpPost("mark-message-read")]
    public async Task<IActionResult> MarkMessageAsRead(Guid id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return BadRequest(new { error = "You must be logged in to perform this action." });
        }

        var result = await _contactMessageClientService.MarkAsReadAsync(id, user.Id);

        if (result == null)
        {
            return NotFound(new { error = "Message not found or you do not have permission to view it." });
        }

        if (result == true)
        {
            return Ok(new { success = "Message marked as read successfully." });
        }

        return Ok(new { info = "Message was already read or has no response." });
    }

    [HttpGet("user-messages")]
    public async Task<IActionResult> GetUserMessages()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return BadRequest(new { error = "You must be logged in to perform this action." });
        }

        var messages = await _contactMessageClientService.GetUserMessagesAsync(user.Id);
        return Ok(messages);
    }
}