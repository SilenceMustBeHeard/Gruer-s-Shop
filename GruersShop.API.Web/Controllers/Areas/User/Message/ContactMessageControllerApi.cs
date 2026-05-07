using GruersShop.Services.Core.Service.Interfaces.Messages;
using GruersShop.Web.ViewModels.Account.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GruersShop.API.Web.Controllers.Areas.User.Message;

[Route("api/[controller]")]
[ApiController]
public class ContactMessageControllerApi : ControllerBase
{
    private readonly IContactMessageClientService _contactMessageService;

    public ContactMessageControllerApi(IContactMessageClientService contactMessageService)
    {
        _contactMessageService = contactMessageService;
    }




    [HttpGet]
    public IActionResult GetForm()
    {
        return Ok(new ContactMessageCreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult>Send([FromBody] ContactMessageCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await _contactMessageService.SendContactMessageAsync(model, User);
        return Ok(new { success = "Your message has been sent successfully! We'll get back to you soon." });

    }
}
