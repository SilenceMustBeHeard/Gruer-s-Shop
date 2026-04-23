using GruersShop.Data.Common.Enums;
using GruersShop.Data.Models.Interactions;
using GruersShop.Data.Repositories.Interfaces.Bakery;
using GruersShop.Data.Repositories.Interfaces.Interactions;
using GruersShop.Services.Core.Service.Interfaces.Interactions;
using GruersShop.Web.ViewModels.Cart;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GruersShop.Services.Core.Service.Implementations.Interactions;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;

    public OrderService(IOrderRepository orderRepository, IProductRepository productRepository)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
    }

    public async Task<Order> CreateOrderFromCartAsync(string userId, CartViewModel cart, string? specialInstructions = null)
    {
        var order = new Order
        {
            UserId = userId,
            OrderDate = DateTime.UtcNow,
            Status = OrderStatus.Pending,
            SpecialInstructions = specialInstructions,
            TotalAmount = cart.TotalAmount
        };

        foreach (var item in cart.Items)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId);
            if (product == null)
                throw new Exception($"Product {item.ProductId} not found");

            if (product.StockQuantity < item.Quantity)
                throw new Exception($"Not enough stock for {product.Name}");

            var orderItem = new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = product.Price
            };

            order.OrderItems.Add(orderItem);

            // Update stock
            product.StockQuantity -= item.Quantity;
            await _productRepository.UpdateAsync(product);
        }

        await _orderRepository.AddAsync(order);
        await _orderRepository.SaveChangesAsync();

        return order;
    }

    public async Task<Order?> GetOrderByIdAsync(Guid orderId)
        => await _orderRepository.GetByIdAsync(orderId);

    public async Task<IEnumerable<Order>> GetUserOrdersAsync(string userId)
        => await _orderRepository
            .Query()
            .Where(o => o.UserId == userId && !o.IsDeleted)
            .Include(o => o.OrderItems)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

    public async Task<Order?> GetOrderWithItemsAsync(Guid orderId)
        => await _orderRepository
            .Query()
            .Where(o => o.Id == orderId && !o.IsDeleted)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync();

    public async Task UpdateOrderStatusAsync(Guid orderId, OrderStatus status)
        => await _orderRepository.UpdateStatusAsync(orderId, status);

    public async Task CancelOrderAsync(Guid orderId)
    {
        var order = await GetOrderWithItemsAsync(orderId);
        if (order == null || order.Status != OrderStatus.Pending)
            throw new Exception("Order cannot be cancelled");

        // Restore stock
        foreach (var item in order.OrderItems)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId);
            if (product != null)
            {
                product.StockQuantity += item.Quantity;
                await _productRepository.UpdateAsync(product);
            }
        }

        order.Status = OrderStatus.Cancelled;
        await _orderRepository.UpdateAsync(order);
        await _orderRepository.SaveChangesAsync();
    }
}