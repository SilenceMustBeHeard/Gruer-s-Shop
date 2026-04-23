using GruersShop.Data.Common.Enums;
using GruersShop.Data.Models.Interactions;
using GruersShop.Web.ViewModels.Cart;

namespace GruersShop.Services.Core.Service.Interfaces.Interactions;

public interface IOrderService
{
    Task<Order> CreateOrderFromCartAsync(string userId, CartViewModel cart, string? specialInstructions = null);
    Task<Order?> GetOrderByIdAsync(Guid orderId);
    Task<IEnumerable<Order>> GetUserOrdersAsync(string userId);
    Task<Order?> GetOrderWithItemsAsync(Guid orderId);
    Task UpdateOrderStatusAsync(Guid orderId, OrderStatus status);
    Task CancelOrderAsync(Guid orderId);
}