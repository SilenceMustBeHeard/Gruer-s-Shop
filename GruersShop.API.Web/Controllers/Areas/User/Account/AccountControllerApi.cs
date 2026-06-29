using GruersShop.Services.Core.Service.Interfaces.Account;
using GruersShop.Web.ViewModels.Account.Profile;
using Microsoft.AspNetCore.Mvc;

namespace GruersShop.API.Web.Controllers.Areas.User.Account;

[Route("api/[controller]")]
[ApiController]
public class AccountControllerApi : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountControllerApi(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpGet("register")]
    public IActionResult Register() => Ok();

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _accountService.RegisterAsync(model);

        if (result.Success)
        {
            return Ok();
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error);
        }

        return BadRequest(ModelState);
    }

    [HttpGet("login")]
    public IActionResult Login() => Ok();

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var success = await _accountService.LoginAsync(model);

        if (!success)
        {
            return Unauthorized();
        }

        return Ok();
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _accountService.LogoutAsync();
        return Ok();
    }
}