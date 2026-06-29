using GruersShop.Services.Core.Service.Interfaces.Interactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GruersShop.API.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class OrdersControllerApi : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersControllerApi(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet("my-orders")]
    public async Task<IActionResult> MyOrders()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var orders = await _orderService.GetUserOrdersAsync(userId);
        return Ok(orders);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderDetails(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { error = "User not authenticated" });
        }

        var order = await _orderService.GetOrderWithItemsAsync(id);

        if (order == null)
        {
            return NotFound(new { error = "Order not found" });
        }

        if (order.UserId != userId)
        {
            return Unauthorized(new { error = "You do not have permission to view this order" });
        }

        return Ok(order);
    }

    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> CancelOrder(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { error = "User not authenticated" });
        }

        var order = await _orderService.GetOrderByIdAsync(id);

        if (order == null)
        {
            return NotFound(new { error = "Order not found" });
        }

        if (order.UserId != userId)
        {
            return Unauthorized(new { error = "You don't have permission to cancel this order" });
        }

        try
        {
            await _orderService.CancelOrderAsync(id);
            return Ok(new
            {
                success = true,
                message = "Order cancelled successfully",
                orderId = id,
                cancelledAt = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                success = false,
                error = ex.Message
            });
        }
    }
}