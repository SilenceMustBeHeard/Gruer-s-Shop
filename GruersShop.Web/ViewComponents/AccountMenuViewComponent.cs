using GruersShop.Data.Models.Base;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GruersShop.Web.ViewComponents;

public class AccountMenuViewComponent : Microsoft.AspNetCore.Mvc.ViewComponent
{
    private readonly SignInManager<AppUser> _signInManager;

    public AccountMenuViewComponent(SignInManager<AppUser> signInManager)
    {
        _signInManager = signInManager;
    }

    public IViewComponentResult Invoke()
    {
        var isLoggedIn = _signInManager.IsSignedIn(HttpContext.User);
        return View(isLoggedIn);
    }
}