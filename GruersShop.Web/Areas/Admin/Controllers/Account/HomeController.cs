using GruersShop.Data.Models.Base;
using GruersShop.Web.ViewModels.Error;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GruersShop.Web.Areas.Admin.Controllers.Account;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class HomeController : BaseAdminController
{
    public HomeController(UserManager<AppUser> userManager) : base(userManager)
    {
    }


    public IActionResult Index() => View();


    public IActionResult About() => View();


    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}