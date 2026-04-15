using GruersShop.Data.Models.Base;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GruersShop.Web.ViewComponents;

public class AccountMenuViewComponent : ViewComponent
{
    private readonly SignInManager<AppUser> _signInManager;

    public AccountMenuViewComponent(SignInManager<AppUser> signInManager)
    {
        _signInManager = signInManager;
    }

    public IViewComponentResult Invoke()
    {
        var user = HttpContext.User;

        var isLoggedIn = _signInManager.IsSignedIn(user);
        var isAdmin = user.IsInRole("Admin");
        var isManager = user.IsInRole("Manager");

        ViewBag.IsLoggedIn = isLoggedIn;
        ViewBag.IsAdmin = isAdmin;
        ViewBag.IsManager = isManager;
        ViewBag.Username = user.Identity?.Name;

        return View();
    }
}