using GruersShop.Data.Models.Base;
using GruersShop.Services.Core.Service.Admin.Interfaces.Message;
using GruersShop.Web.ViewModels.Admin.Message;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GruersShop.API.Web.Controllers.Areas.Admin.Message;

[Route("api/admin/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class ContactMessageControllerApi : ControllerBase
{
    private readonly IContactMessageService _contactMessageService;
    private readonly UserManager<AppUser> _userManager;

    public ContactMessageControllerApi(
        IContactMessageService contactMessageService,
        UserManager<AppUser> userManager)
    {
        _contactMessageService = contactMessageService;
        _userManager = userManager;
    }

    [HttpGet("messages")]
    public async Task<IActionResult> GetMessages()
    {
        var adminId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(adminId))
        {
            return Unauthorized(new { message = "You must be logged in." });
        }

        var messages = await _contactMessageService.GetAdminMessagesAsync(adminId);

      
        return Ok(messages ?? new List<ContactMessageDetailsViewModel>());
    }

    [HttpGet("details/{id}")]
    public async Task<IActionResult> GetDetails(Guid id)
    {
        var adminId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(adminId))
        {
            return Unauthorized(new { message = "You must be logged in." });
        }

        var message = await _contactMessageService.GetMessageDetailsAsync(id, adminId);

        if (message == null)
        {
            return NotFound(new { message = "Message not found or you do not have permission to view it." });
        }

        if (!message.IsReadByAdmin)
        {
            await _contactMessageService.MarkMessageAsReadAsync(id, adminId);
            message.IsReadByAdmin = true;
        }

        return Ok(message);
    }

    [HttpGet("respond/{id}")]
    public async Task<IActionResult> GetRespondForm(Guid id)
    {
        var adminId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(adminId))
        {
            return Unauthorized(new { message = "User not found." });
        }

        var message = await _contactMessageService.GetMessageDetailsAsync(id, adminId);

        if (message == null)
        {
            return NotFound(new { message = "Message not found." });
        }

        if (!string.IsNullOrEmpty(message.Response))
        {
            return BadRequest(new { message = "This message has already been responded to." });
        }

        return Ok(new ContactMessageResponseViewModel
        {
            Id = message.Id,
            Subject = message.Subject,
            SenderName = message.SenderName,        
            SenderEmail = message.SenderEmail,
            OriginalMessage = message.Message,
            Response = string.Empty
        });
    }

    [HttpPost("respond")]
    public async Task<IActionResult> SendResponse([FromBody] ContactMessageResponseViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var adminId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(adminId))
        {
            return Unauthorized(new { message = "User not found." });
        }

        try
        {
            await _contactMessageService.RespondToMessageAsync(model.Id, model.Response, adminId);
            return Ok(new { success = true, message = "Response sent successfully!" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount()
    {
        var adminId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(adminId))
        {
            return Unauthorized(new { message = "User not found." });
        }

        var unreadCount = await _contactMessageService.GetUnreadCountAsync(adminId);

        
        return Ok(new { unreadCount });
    }

    [HttpGet("messages-partial")]
    public async Task<IActionResult> GetMessagesPartial()
    {
        var adminId = _userManager.GetUserId(User);

       
        if (string.IsNullOrEmpty(adminId))
        {
            return Unauthorized(new { message = "You must be logged in." });
        }

        var messages = await _contactMessageService.GetAdminMessagesAsync(adminId);

        return Ok(messages ?? new List<ContactMessageDetailsViewModel>());
    }

    [HttpPost("mark-all-read")]
    public async Task<IActionResult> MarkAllAsRead()
    {
        var adminId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(adminId))
        {
            return Unauthorized(new { message = "You must be logged in." });
        }

        await _contactMessageService.MarkAllMessagesAsReadAsync(adminId);

        return Ok(new { success = true, message = "All messages marked as read." });
    }
}