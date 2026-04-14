using GruersShop.Data.Models.Base;
using GruersShop.Services.Core.Service.Admin.Interfaces.Message;
using GruersShop.Web.ViewModels.Admin.Message;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GruersShop.Web.Areas.Admin.Message;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ContactMessageController : Controller
{
    private readonly IContactMessageService _contactMessageService;
    private readonly UserManager<AppUser> _userManager;

    public ContactMessageController(
        IContactMessageService contactMessageService,
        UserManager<AppUser> userManager)
    {
        _contactMessageService = contactMessageService;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var adminId = _userManager.GetUserId(User);
        var messages = await _contactMessageService.GetAdminMessagesAsync(adminId);
        return View(messages);
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        if (id == Guid.Empty)
        {
            TempData["Error"] = "Invalid message ID.";
            return BadRequest();
        }

        var adminId = _userManager.GetUserId(User);
        if (adminId == null)
        {
            TempData["Error"] = "User not found.";
            return NotFound();
        }
        var message = await _contactMessageService.GetMessageDetailsAsync(id, adminId);

        if (message == null)
        {
            TempData["Error"] = "Message not found.";
            return NotFound();
        }

        return View(message);
    }

    [HttpGet]
    public async Task<IActionResult> Respond(Guid id)
    {
        var adminId = _userManager.GetUserId(User);
        if (adminId == null)
        {
            TempData["Error"] = "User not found.";
            return NotFound();
        }
        var message = await _contactMessageService.GetMessageDetailsAsync(id, adminId);

        if (message == null)
        {
            TempData["Error"] = "Message not found.";
            return NotFound();
        }

        if (!string.IsNullOrEmpty(message.Response))
        {
            TempData["Error"] = "This message has already been responded to.";
            return RedirectToAction(nameof(Details), new { id });
        }
        if (message.Response != null)
        {
            TempData["Error"] = "This message has already been responded to.";
            return RedirectToAction(nameof(Details), new { id });
        }

        var model = new ContactMessageResponseViewModel
        {
            Id = message.Id,
            Subject = message.Subject,
            SenderName = message.SenderName,
            SenderEmail = message.SenderEmail,
            OriginalMessage = message.Message,
            Response = string.Empty
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Respond(ContactMessageResponseViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var adminId = _userManager.GetUserId(User);
        if (adminId == null)
        {
            TempData["Error"] = "User not found.";
            return NotFound();
        }

        try
        {
            await _contactMessageService.RespondToMessageAsync(model.Id, model.Response, adminId);
            TempData["Success"] = "Response sent successfully!";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Details), new { id = model.Id });
        }

        return RedirectToAction(nameof(Details), new { id = model.Id });
    }
}