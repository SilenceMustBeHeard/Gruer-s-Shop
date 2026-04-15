using GruersShop.Data.Models.Base;
using GruersShop.Data.Models.Messages;
using GruersShop.Data.Repositories.Interfaces.Account;
using GruersShop.Services.Core.Service.Admin.Interfaces.Message;
using GruersShop.Web.ViewModels.Admin.Message;
using GruersShop.Web.ViewModels.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GruersShop.Web.Areas.Admin.Controllers.Message;


[Area("Admin")]
[Authorize(Roles = "Admin")]
public class SystemMessageController : Controller
{
    private readonly ISystemInboxMessageService _systemMessageService;
    private readonly IAppUserRepository _userRepository;
    private readonly UserManager<AppUser> _userManager;

    public SystemMessageController(
        ISystemInboxMessageService systemMessageService,
        IAppUserRepository userRepository,
        UserManager<AppUser> userManager)
    {
        _systemMessageService = systemMessageService;
        _userRepository = userRepository;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var adminId = _userManager.GetUserId(User);
        var messages = await _systemMessageService.GetAdminMessagesAsync(adminId);
        return View(messages);
    }

    [HttpGet]
    public async Task<IActionResult> Create(string userId)
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

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SystemInboxMessageCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Please correct the errors in the form.";
            return View(model);
        }

        model.AvailableUsers = await _userRepository
            .Query()
            .Select(u => new UserSelectViewModel
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email ?? string.Empty
            })
            .ToListAsync();

        if (!string.IsNullOrEmpty(model.ReceiverId))
        {
            var selectedUser = model.AvailableUsers.FirstOrDefault(u => u.Id == model.ReceiverId);
            if (selectedUser != null)
            {
                model.ReceiverName = selectedUser.FullName;
            }
        }

        var adminId = _userManager.GetUserId(User);

        if (adminId == null)
        {
            TempData["Error"] = "You are not authorized to send messages.";
            return BadRequest();
        }

        var receiver = await _userManager.FindByIdAsync(model.ReceiverId);
        if (receiver == null)
        {
            TempData["Error"] = "Receiver not found.";
            return View(model);
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

        TempData["Success"] = "Message sent successfully!";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var adminId = _userManager.GetUserId(User);

        if (adminId == null)
        {
            TempData["Error"] = "You are not authorized to view this message.";
            return BadRequest();
        }
        var message = await _systemMessageService.GetMessageDetailsAsync(id, adminId);

        if (message == null)
        {
            TempData["Error"] = "Message not found or you don't have permission to view it.";
            return NotFound();
        }

        var sender = await _userManager.FindByIdAsync(message.SenderId ?? "");
        var receiver = await _userManager.FindByIdAsync(message.ReceiverId!);

        message.SenderName = sender != null ? $"{sender.FullName}" : "System";
        message.ReceiverName = receiver != null ? $"{receiver.FullName}" : "Unknown";

        return View(message);
    }
}