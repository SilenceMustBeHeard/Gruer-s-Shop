using GruersShop.Web.ViewModels.Orders;
using GruersShop.Data.Common.Enums;
using GruersShop.Services.Core.Service.Interfaces.Interactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GruersShop.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin,Manager")]
public class OrderManagementController : Controller
{
    private readonly IOrderManagementService _orderManagementService;

    public OrderManagementController(IOrderManagementService orderManagementService)
    {
        _orderManagementService = orderManagementService;
    }

    public async Task<IActionResult> Index(OrderStatus? status = null)
    {
        var orders = await _orderManagementService.GetAllOrdersAsync(status);
        ViewBag.SelectedStatus = status;
        return View(orders);
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var order = await _orderManagementService.GetOrderWithDetailsAsync(id);
        if (order == null)
        {
            TempData["Error"] = "Order not found.";
            return RedirectToAction(nameof(Index));
        }

        var stockCheck = await _orderManagementService.CheckStockAvailabilityAsync(order);
        ViewBag.StockIssues = stockCheck.Issues;
        ViewBag.CanFulfill = stockCheck.IsAvailable;

        return View(order);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(Guid id, OrderStatus newStatus, string? reason = null, int? delayDays = null)
    {
        OrderUpdateResult result = newStatus == OrderStatus.Cancelled && !string.IsNullOrEmpty(reason)
            ? await _orderManagementService.CancelOrderAsync(id, reason)
            : await _orderManagementService.UpdateOrderStatusAsync(id, newStatus, delayDays);

        if (result.Success)
            TempData["Success"] = result.Message;
        else
            TempData["Error"] = result.Message;

        return RedirectToAction(nameof(Details), new { id });
    }
}