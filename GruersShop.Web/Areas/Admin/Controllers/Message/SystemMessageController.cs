using GruersShop.Data.Models.Base;
using GruersShop.Services.Core.Service.Admin.Interfaces.Message;
using GruersShop.Web.ViewModels.Admin.Message;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GruersShop.Web.Areas.Admin.Controllers.Message;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class SystemMessageController : Controller
{
    private readonly ISystemInboxMessageService _systemMessageService;
    private readonly UserManager<AppUser> _userManager;

    public SystemMessageController(
        ISystemInboxMessageService systemMessageService,
        UserManager<AppUser> userManager)
    {
        _systemMessageService = systemMessageService;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var adminId = _userManager.GetUserId(User);
        var messages = await _systemMessageService.GetAdminMessagesAsync(adminId!);
        return View(messages);
    }

    [HttpGet]
    public async Task<IActionResult> Create(string? userId)
    {
        var model = await _systemMessageService.GetCreateViewModelAsync(userId);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SystemInboxMessageCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.AvailableUsers = (await _systemMessageService.GetCreateViewModelAsync()).AvailableUsers;
            TempData["Error"] = "Please correct the errors in the form.";
            return View(model);
        }

        var adminId = _userManager.GetUserId(User);
        if (adminId == null)
        {
            TempData["Error"] = "You are not authorized to send messages.";
            return Unauthorized();
        }

        var (success, errorMessage) = await _systemMessageService.CreateMessageAsync(model, adminId);

        if (!success)
        {
            TempData["Error"] = errorMessage;
            model.AvailableUsers = (await _systemMessageService.GetCreateViewModelAsync()).AvailableUsers;
            return View(model);
        }

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
            return Unauthorized();
        }

        var message = await _systemMessageService.GetMessageDetailsAsync(id, adminId);
        if (message == null)
        {
            TempData["Error"] = "Message not found or you don't have permission to view it.";
            return NotFound();
        }

        return View(message);
    }
}