using GruersShop.Data.Common.Enums;
using GruersShop.Data.Models.Interactions;
using GruersShop.Data.Repositories.Interfaces.CRUD;

namespace GruersShop.Data.Repositories.Interfaces.Interactions;

public interface IOrderRepository
     : IFullRepositoryAsync<Order, Guid>
{
    Task<int> CountPendingAsync();

    Task<Order?> GetOrderWithProductsAsync(Guid orderId);

    Task<Order?> GetOrderWithDetailsAsync(Guid orderId);

    Task UpdateStatusAsync(Guid orderId, OrderStatus newStatus);
}