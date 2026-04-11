using GruersShop.Data.Common.Enums;
using GruersShop.Data.Models.Interactions;
using GruersShop.Data.Repositories.Implementations.Base;
using GruersShop.Data.Repositories.Interfaces.CRUD;
using System;
using System.Collections.Generic;
using System.Text;

namespace GruersShop.Data.Repositories.Interfaces.Interactions;

public interface IOrderRepository
     : IFullRepositoryAsync<Order, Guid>
{
    Task<int> CountPendingAsync();
    Task<Order?> GetOrderWithVariantsAsync(Guid orderId);


    Task UpdateStatusAsync(Guid orderId, OrderStatus newStatus);
}