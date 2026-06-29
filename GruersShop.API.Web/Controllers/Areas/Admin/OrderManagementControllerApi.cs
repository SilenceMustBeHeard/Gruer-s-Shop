using GruersShop.Data.Common.Enums;
using GruersShop.Data.Models.Interactions;
using GruersShop.Services.Core.Service.Interfaces.Interactions;
using GruersShop.Web.ViewModels.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GruersShop.API.Web.Controllers.Areas.Admin;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class OrderManagementControllerApi : ControllerBase
{
    private readonly IOrderManagementService _orderManagementService;

    public OrderManagementControllerApi(IOrderManagementService orderManagementService)
    {
        _orderManagementService = orderManagementService;
    }

    [HttpGet("orders")]
    public async Task<IActionResult> Orders(OrderStatus? status = null, bool includeDeleted = false)
    {
        var orders = await _orderManagementService.GetAllOrdersAsync(status);

        if (orders == null || !orders.Any())
        {
            return Ok(new List<Order>());
        }

        if (!includeDeleted)
        {
            orders = orders.Where(o => !o.IsDeleted);
        }

        return Ok(orders);
    }

    [HttpGet("details/{id}")]
    public async Task<IActionResult> Details(Guid id)
    {
        var order = await _orderManagementService.GetOrderWithDetailsAsync(id);
        if (order == null)
        {
            return NotFound(new { error = "Order not found." });
        }

        return Ok(order);
    }

    [HttpPost("{id}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateStatusRequest request)
    {
        OrderUpdateResult result = request.NewStatus == OrderStatus.Cancelled && !string.IsNullOrEmpty(request.Reason)
            ? await _orderManagementService.CancelOrderAsync(id, request.Reason)
            : await _orderManagementService.UpdateOrderStatusAsync(id, request.NewStatus, request.DelayDays);

        if (result.Success)
        {
            return Ok(new { success = true, message = result.Message });
        }

        return BadRequest(new { success = false, message = result.Message });
    }

    [HttpPost("toggle-active/{orderId}")]
    public async Task<IActionResult> ToggleActive(Guid orderId)
    {
        try
        {
            await _orderManagementService.ToggleOrderAsync(orderId);

            var order = await _orderManagementService.GetByIdAsync(orderId);

            if (order != null)
            {
                return Ok(new
                {
                    success = true,
                    message = order.IsDeleted
                    ? "🔒 Order has been hidden from customers!"
                    : "✨ Order is now visible to customers!"
                });
            }
            else
            {
                return Ok(new { success = true, message = "Order status changed successfully!" });
            }
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = "Error toggling order status: " + ex.Message });
        }
    }

    public class UpdateStatusRequest
    {
        public OrderStatus NewStatus { get; set; }
        public string? Reason { get; set; }
        public int? DelayDays { get; set; }
    }
}