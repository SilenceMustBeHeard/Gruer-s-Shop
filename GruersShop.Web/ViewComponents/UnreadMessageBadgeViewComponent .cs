using GruersShop.Services.Core.Service.Interfaces.Messages;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace GruersShop.Web.ViewComponents;

public class UnreadMessageBadgeViewComponent : ViewComponent
{
    private readonly IContactMessageClientService _service;

    public UnreadMessageBadgeViewComponent(IContactMessageClientService service)
    {
        _service = service;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var principal = HttpContext.User;

        if (!(principal?.Identity?.IsAuthenticated ?? false))
        {
            return View(0);
        }

        var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(userId))
        {
            return View(0);
        }

        try
        {
            var count = await _service.GetUserUnreadResponsesCountAsync(userId);
            return View(count);
        }
        catch
        {
            return View(0);
        }
    }

}