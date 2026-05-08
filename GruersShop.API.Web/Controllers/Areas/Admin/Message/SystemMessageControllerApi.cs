using GruersShop.Data.Models.Base;
using GruersShop.Data.Models.Messages;
using GruersShop.Data.Repositories.Interfaces.Account;
using GruersShop.Services.Core.Service.Admin.Interfaces.Message;
using GruersShop.Web.ViewModels.Account.Messages;
using GruersShop.Web.ViewModels.Admin.Message;
using GruersShop.Web.ViewModels.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GruersShop.API.Web.Controllers.Areas.Admin.Message;

[Route("api/admin/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class SystemMessageControllerApi : ControllerBase
{
    private readonly ISystemInboxMessageService _systemMessageService;
    private readonly IAppUserRepository _userRepository;
    private readonly UserManager<AppUser> _userManager;

    public SystemMessageControllerApi(
        ISystemInboxMessageService systemMessageService,
        IAppUserRepository userRepository,
        UserManager<AppUser> userManager)
    {
        _systemMessageService = systemMessageService;
        _userRepository = userRepository;
        _userManager = userManager;
    }

    [HttpGet("messages")]
    public async Task<IActionResult> GetMessages()
    {
        var adminId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(adminId))
        {
            return Unauthorized(new { error = "You are not authorized to view messages." });
        }

        var messages = await _systemMessageService.GetAdminMessagesAsync(adminId);
        return Ok(messages ?? new List<SystemInboxMessageViewModel>());
    }

    [HttpGet("create-form")]
    public async Task<IActionResult> GetCreateForm([FromQuery] string? userId)
    {
        var model = new SystemInboxMessageCreateViewModel
        {
            ReceiverId = userId,
            AvailableUsers = await _userRepository
                .Query()
                //.Where(u => !u.IsDeleted)
                .Select(u => new UserSelectViewModel
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email ?? string.Empty
                })
                .ToListAsync()
        };

        if (!string.IsNullOrEmpty(userId))
        {
            var selectedUser = model.AvailableUsers.FirstOrDefault(u => u.Id == userId);
            if (selectedUser != null)
            {
                model.ReceiverName = selectedUser.FullName;
            }
        }

        return Ok(model);
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendMessage([FromBody] SystemInboxMessageCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
        }

        var adminId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(adminId))
        {
            return Unauthorized(new { error = "You are not authorized to send messages." });
        }

        var receiver = await _userManager.FindByIdAsync(model.ReceiverId);
        if (receiver == null)
        {
            return BadRequest(new { error = "Receiver not found." });
        }

        var message = new SystemInboxMessage
        {
            Id = Guid.NewGuid(),
            SenderId = adminId,
            ReceiverId = model.ReceiverId,
            Receiver = receiver,
            Title = model.Title,
            Description = model.Description,
            Type = model.Type,
            CreatedAt = DateTime.UtcNow
        };

        await _systemMessageService.CreateMessageAsync(message);

        return Ok(new
        {
            success = true,
            message = "Message sent successfully!",
            messageId = message.Id,
            sentAt = message.CreatedAt
        });
    }

    [HttpGet("details/{id}")]
    public async Task<IActionResult> GetMessageDetails(Guid id)
    {
        var adminId = _userManager.GetUserId(User);

        if (string.IsNullOrEmpty(adminId))
        {
            return Unauthorized(new { error = "You are not authorized to view this message." });
        }

        var message = await _systemMessageService.GetMessageDetailsAsync(id, adminId);

        if (message == null)
        {
            return NotFound(new { error = "Message not found or you don't have permission to view it." });
        }

     
        var sender = await _userManager.FindByIdAsync(message.SenderId ?? "");
        var receiver = await _userManager.FindByIdAsync(message.ReceiverId ?? "");

        message.SenderName = sender?.FullName ?? "System";
        message.ReceiverName = receiver?.FullName ?? "Unknown User";

        return Ok(message);
    }

    [HttpGet("available-users")]
    public async Task<IActionResult> GetAvailableUsers()
    {
        var users = await _userRepository
            .Query()
          //  .Where(u => !u.IsDeleted)
            .Select(u => new UserSelectViewModel
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email ?? string.Empty
            })
            .ToListAsync();

        return Ok(new
        {
            users = users,
            totalCount = users.Count
        });
    }
}