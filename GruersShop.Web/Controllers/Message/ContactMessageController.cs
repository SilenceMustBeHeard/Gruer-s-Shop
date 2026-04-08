using GruersShop.Services.Core.Service.Interfaces.Messages;
using GruersShop.Web.ViewModels.Account.Messages;
using Microsoft.AspNetCore.Mvc;

namespace GruersShop.Web.Controllers.Message;

public class ContactMessageController : Controller
{
    private readonly IContactMessageClientService _contactMessageService;

    public ContactMessageController(IContactMessageClientService contactMessageService)
    {
        _contactMessageService = contactMessageService;
    }




    [HttpGet]
    public IActionResult Index()
    {
        return View(new ContactMessageCreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(ContactMessageCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        await _contactMessageService.SendContactMessageAsync(model, User);
        TempData["Success"] = "Your message has been sent successfully! We'll get back to you soon.";
        return RedirectToAction("Index", "Home");

    }
}