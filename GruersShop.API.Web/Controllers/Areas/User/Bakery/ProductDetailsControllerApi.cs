using GruersShop.Services.Core.Service.Interfaces.Bakery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GruersShop.API.Web.Controllers.Areas.User.Bakery;

[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]
public class ProductDetailsControllerApi : ControllerBase
{
    private readonly ICatalogClientService _catalogClientService;

    public ProductDetailsControllerApi(ICatalogClientService catalogClientService)
    {
        _catalogClientService = catalogClientService;
    }

    [HttpGet]
    public virtual async Task<IActionResult> Index(Guid id)
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var model = await _catalogClientService.GetProductDetailsViewModelAsync(id, userId);

        if (model == null)
        {
            return NotFound();
        }

        return Ok(model);
    }
}