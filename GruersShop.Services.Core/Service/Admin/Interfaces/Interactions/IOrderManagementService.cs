using GruersShop.Data.Common.Enums;

using GruersShop.Data.Models.Interactions;
using GruersShop.Web.ViewModels.Interactions;
using GruersShop.Web.ViewModels.Orders;

namespace GruersShop.Services.Core.Service.Interfaces.Interactions;

public interface IOrderManagementService
{
    Task<IEnumerable<Order>> GetAllOrdersAsync(OrderStatus? status = null);
    Task<Order?> GetOrderWithDetailsAsync(Guid orderId);
    Task<StockCheckResult> CheckStockAvailabilityAsync(Order order);
    Task<OrderUpdateResult> ConfirmOrderAsync(Guid orderId);
    Task<OrderUpdateResult> UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus, int? daysDelay = null);
    Task<OrderUpdateResult> CancelOrderAsync(Guid orderId, string reason);

     Task<bool> HasRecentUpdatesAsync(string userId, DateTime since);

    Task<OrderViewModel?> GetByIdAsync(Guid orderId);
    // Status management
    Task ToggleOrderAsync(Guid orderId);



}