using GruersShop.Data.Models;
using GruersShop.Web.ViewModels.Error;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GruersShop.Web.Controllers.Account;

public class HomeController : BaseController
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager)
        : base(userManager)
    {
        _logger = logger;
    }

    // The home page, about page, and privacy page are accessible to all users, including anonymous ones.
    [AllowAnonymous]
    public IActionResult Index() => View();

    [AllowAnonymous]
    public IActionResult About() => View();

    [AllowAnonymous]
    public IActionResult Privacy() => View();


    // The error page is also accessible to all users, and it displays the request ID for debugging purposes.

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
