using GruersShop.Data.Common.Enums;
using GruersShop.Data.Models.Interactions;
using GruersShop.Data.Repositories.Implementations.Base;
using GruersShop.Data.Repositories.Interfaces.Interactions;
using Microsoft.EntityFrameworkCore;

namespace GruersShop.Data.Repositories.Implementations.Interactions;

public class OrderRepository
       : RepositoryAsync<Order, Guid>, IOrderRepository
{
    public OrderRepository(AppDbContext context)
        : base(context)
    {
    }

    public async Task<int> CountPendingAsync()
    {
        return await _dbSet
            .CountAsync(o => o.Status == OrderStatus.Pending);
    }

    // retrieves an order along with its associated products

    public async Task<Order?> GetOrderWithProductsAsync(Guid orderId)
    {
        return await _dbSet
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == orderId);
    }

    // updates the status of an order

    public async Task UpdateStatusAsync(Guid orderId, OrderStatus newStatus)
    {
        var order = await _dbSet.FindAsync(orderId);
        if (order != null)
        {
            order.Status = newStatus;
            _dbSet.Update(order);
            await _context.SaveChangesAsync();
        }
    }
}