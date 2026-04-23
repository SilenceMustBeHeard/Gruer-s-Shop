using GruersShop.Services.Core.Service.Interfaces.Interactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GruersShop.Web.Controllers;

[Authorize]
public class OrdersController : Controller
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public async Task<IActionResult> MyOrders()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var orders = await _orderService.GetUserOrdersAsync(userId);
        return View(orders);
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var order = await _orderService.GetOrderWithItemsAsync(id);

        if (order == null || order.UserId != userId)
            return NotFound();

        return View(order);
    }

    [HttpPost]
    public async Task<IActionResult> Cancel(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var order = await _orderService.GetOrderByIdAsync(id);

        if (order == null || order.UserId != userId)
            return NotFound();

        try
        {
            await _orderService.CancelOrderAsync(id);
            TempData["Success"] = "Order cancelled successfully";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction("Details", new { id });
    }
}