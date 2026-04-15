using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GruersShop.Web.Controllers;

[Authorize]
public class OrdersController : Controller
{
    public IActionResult MyOrders() => View();
    public IActionResult Details(Guid id) => View();
}