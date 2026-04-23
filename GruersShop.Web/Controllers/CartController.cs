using GruersShop.Services.Core.Service.Interfaces.Interactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GruersShop.Web.Controllers;

[Authorize]
public class CartController : Controller
{
    private readonly ICartService _cartService;
    private readonly IOrderService _orderService;

    public CartController(ICartService cartService, IOrderService orderService)
    {
        _cartService = cartService;
        _orderService = orderService;
    }

    public IActionResult Index()
    {
        var cart = _cartService.GetCart();
        return View(cart);
    }

    [HttpPost]
    public async Task<IActionResult> AddToCart(Guid id, int quantity = 1)
    {
        await _cartService.AddToCartAsync(id, quantity);
        TempData["Success"] = "Product added to cart!";
        return RedirectToAction("Index", "ProductDetails", new { id });
    }

    [HttpPost]
    public async Task<IActionResult> UpdateQuantity(Guid productId, int quantity)
    {
        await _cartService.UpdateQuantityAsync(productId, quantity);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> RemoveFromCart(Guid productId)
    {
        await _cartService.RemoveFromCartAsync(productId);
        TempData["Success"] = "Product removed from cart";
        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Checkout()
    {
        var cart = _cartService.GetCart();
        if (!cart.Items.Any())
        {
            TempData["Error"] = "Your cart is empty";
            return RedirectToAction("Index");
        }
        return View(cart);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout(string? pickupDate, string? pickupTime, string? specialInstructions)
    {
        var cart = _cartService.GetCart();
        if (!cart.Items.Any())
        {
            TempData["Error"] = "Your cart is empty";
            return RedirectToAction("Index");
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        try
        {
            var order = await _orderService.CreateOrderFromCartAsync(userId, cart, pickupDate, pickupTime, specialInstructions);
            await _cartService.ClearCartAsync();
            TempData["Success"] = "Order placed successfully!";
            return RedirectToAction("Details", "Orders", new { id = order.Id });
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction("Checkout");
        }
    }
}