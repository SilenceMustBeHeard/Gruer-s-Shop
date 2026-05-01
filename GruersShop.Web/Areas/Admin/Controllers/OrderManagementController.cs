using GruersShop.Data.Common.Enums;
using GruersShop.Services.Core.Service.Interfaces.Interactions;
using GruersShop.Web.ViewModels.Interactions;
using GruersShop.Web.ViewModels.Orders;
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

    public async Task<IActionResult> Index(OrderStatus? status = null, bool includeDeleted = false)
    {
        var orders = await _orderManagementService.GetAllOrdersAsync(status);

        if (!includeDeleted)
        {
            orders = orders.Where(o => !o.IsDeleted);
        }

        ViewBag.SelectedStatus = status;
        ViewBag.IncludeDeleted = includeDeleted;
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



  



    [HttpPost]
    [ValidateAntiForgeryToken]
   
    public async Task<IActionResult> ToggleActive(Guid orderId)
    {
        try
        {

            await _orderManagementService.ToggleOrderAsync(orderId);


            var order = await _orderManagementService.GetByIdAsync(orderId);

            if (order != null)
            {
                TempData["Success"] = order.IsDeleted
                    ? "🔒 Order has been hidden from customers!"
                    : "✨ Order is now visible to customers!";   
            }
            else
            {
                TempData["Success"] = "Order status changed successfully!";
            }
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Error toggling order status: " + ex.Message;
        }

        return RedirectToAction("Index");
    }







}