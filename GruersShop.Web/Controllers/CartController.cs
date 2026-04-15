using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GruersShop.Web.Controllers;

[Authorize]
public class CartController : Controller
{
   
    public IActionResult Index() => View();
    public IActionResult AddToCart(Guid id) => RedirectToAction("Index");
    public IActionResult Checkout() => View();
}