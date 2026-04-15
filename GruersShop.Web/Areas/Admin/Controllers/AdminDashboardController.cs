using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GruersShop.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin,Manager")]
public class AdminDashboardController : Controller
{
    public IActionResult Index()
    {
        var isAdmin = User.IsInRole("Admin");
        var isManager = User.IsInRole("Manager");

        ViewBag.IsAdmin = isAdmin;
        ViewBag.IsManager = isManager;

        return View();
    }
}