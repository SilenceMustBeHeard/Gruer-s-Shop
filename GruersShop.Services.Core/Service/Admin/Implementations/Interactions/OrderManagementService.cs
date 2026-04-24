
using GruersShop.Data.Common.Enums;
using GruersShop.Data.Models.Base;
using GruersShop.Data.Models.Interactions;
using GruersShop.Data.Models.Messages;
using GruersShop.Data.Repositories.Interfaces.Bakery;
using GruersShop.Data.Repositories.Interfaces.Interactions;
using GruersShop.Services.Core.Service.Admin.Interfaces.Message;
using GruersShop.Services.Core.Service.Interfaces.Interactions;
using GruersShop.Web.ViewModels.Orders;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GruersShop.Services.Core.Service.Implementations.Interactions;

public class OrderManagementService : IOrderManagementService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly UserManager<AppUser> _userManager;
    private readonly ISystemInboxMessageService _systemMessageService;

    public OrderManagementService(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        ISystemInboxMessageService systemMessageService,
        UserManager<AppUser> userManager)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _systemMessageService = systemMessageService;
        _userManager = userManager;
    }

    public async Task<IEnumerable<Order>> GetAllOrdersAsync(OrderStatus? status = null)
    {
        IQueryable<Order> query = _orderRepository
            .Query()
            .Include(o => o.User)
            .Include(o => o.OrderItems)
            .OrderByDescending(o => o.OrderDate);

        if (status.HasValue)
        {
            query = query.Where(o => o.Status == status.Value);
        }

        return await query.ToListAsync();
    }

    public async Task<Order?> GetOrderWithDetailsAsync(Guid orderId)
    {
        return await _orderRepository
            .Query()
            .Include(o => o.User)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == orderId);
    }

    public async Task<StockCheckResult> CheckStockAvailabilityAsync(Order order)
    {
        var result = new StockCheckResult { IsAvailable = true };

        foreach (var item in order.OrderItems)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId);
            if (product == null || product.StockQuantity < item.Quantity)
            {
                result.IsAvailable = false;
                result.Issues.Add(new StockIssue
                {
                    ProductName = product?.Name ?? "Unknown",
                    RequestedQuantity = item.Quantity,
                    AvailableStock = product?.StockQuantity ?? 0
                });
            }
        }

        return result;
    }

    public async Task<OrderUpdateResult> ConfirmOrderAsync(Guid orderId)
    {
        var order = await GetOrderWithDetailsAsync(orderId);
        if (order == null)
            return new OrderUpdateResult { Success = false, Message = "Order not found." };

        if (order.Status != OrderStatus.Pending)
            return new OrderUpdateResult { Success = false, Message = "Order cannot be confirmed at this stage." };

        var stockCheck = await CheckStockAvailabilityAsync(order);
        if (!stockCheck.IsAvailable)
        {
            return new OrderUpdateResult
            {
                Success = false,
                Message = $"Cannot confirm order: {string.Join(", ", stockCheck.Issues.Select(i => i.ProductName))}"
            };
        }

        foreach (var item in order.OrderItems)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId);
            if (product != null)
            {
                product.StockQuantity -= item.Quantity;
                await _productRepository.UpdateAsync(product);
            }
        }

        order.Status = OrderStatus.Confirmed;
        await _orderRepository.UpdateAsync(order);
        await _orderRepository.SaveChangesAsync();

        return new OrderUpdateResult { Success = true, Message = "Order confirmed successfully.", Order = order };
    }

    public async Task<OrderUpdateResult> UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus, int? daysDelay = null)
    {
        var order = await GetOrderWithDetailsAsync(orderId);
        if (order == null)
            return new OrderUpdateResult { Success = false, Message = "Order not found." };

        if (!IsValidStatusTransition(order.Status, newStatus))
            return new OrderUpdateResult { Success = false, Message = $"Cannot change status from {order.Status} to {newStatus}." };

        if (newStatus == OrderStatus.Confirmed && order.Status == OrderStatus.Pending)
        {
            var confirmResult = await ConfirmOrderAsync(orderId);
            if (!confirmResult.Success)
                return confirmResult;
            order = confirmResult.Order;
        }

        if (newStatus == OrderStatus.Cancelled)
        {
            await RestoreStockAsync(order);
        }

        if (daysDelay.HasValue && daysDelay.Value > 0 && order.PickupDate.HasValue)
        {
            order.PickupDate = order.PickupDate.Value.AddDays(daysDelay.Value);
        }

        order.Status = newStatus;
        await _orderRepository.UpdateAsync(order);
        await _orderRepository.SaveChangesAsync();

        return new OrderUpdateResult { Success = true, Message = $"Order status updated to {newStatus}.", Order = order };
    }

    public async Task<OrderUpdateResult> CancelOrderAsync(Guid orderId, string reason)
    {
        var result = await UpdateOrderStatusAsync(orderId, OrderStatus.Cancelled);
        if (result.Success && result.Order != null)
        {
            await NotifyCustomerAsync(result.Order, reason);
        }
        return result;
    }

    private async Task RestoreStockAsync(Order order)
    {
        foreach (var item in order.OrderItems)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId);
            if (product != null)
            {
                product.StockQuantity += item.Quantity;
                await _productRepository.UpdateAsync(product);
            }
        }
    }

    private async Task NotifyCustomerAsync(Order order, string message)
    {
        var customer = await _userManager.FindByIdAsync(order.UserId);

        var adminUser = (await _userManager.GetUsersInRoleAsync("Admin")).FirstOrDefault();


        if (customer == null
            || adminUser == null) return;

        var reciever = await _userManager.FindByIdAsync(customer.Id);

        var systemMessage = new SystemInboxMessage
        {
            Id = Guid.NewGuid(),
            Title = "📦 Order Cancelled",
            Description = message,
            SenderId = adminUser.Id,
            ReceiverId = customer.Id,
            Receiver = reciever,
            Type = InboxMessageType.AdminToUser,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        await _systemMessageService.CreateMessageAsync(systemMessage);
    }

    private bool IsValidStatusTransition(OrderStatus current, OrderStatus next)
    {
        return (current, next) switch
        {
            (OrderStatus.Pending, OrderStatus.Confirmed) => true,
            (OrderStatus.Pending, OrderStatus.Cancelled) => true,
            (OrderStatus.Confirmed, OrderStatus.Baking) => true,
            (OrderStatus.Confirmed, OrderStatus.Cancelled) => true,
            (OrderStatus.Baking, OrderStatus.ReadyForPickup) => true,
            (OrderStatus.Baking, OrderStatus.Cancelled) => true,
            (OrderStatus.ReadyForPickup, OrderStatus.Completed) => true,
            (OrderStatus.ReadyForPickup, OrderStatus.Cancelled) => true,
            _ => false
        };
    }
}