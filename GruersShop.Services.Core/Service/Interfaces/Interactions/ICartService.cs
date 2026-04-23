using GruersShop.Web.ViewModels.Cart;

namespace GruersShop.Services.Core.Service.Interfaces.Interactions;

public interface ICartService
{
    CartViewModel GetCart();
    Task AddToCartAsync(Guid productId, int quantity = 1);
    Task RemoveFromCartAsync(Guid productId);
    Task UpdateQuantityAsync(Guid productId, int quantity);
    Task ClearCartAsync();
    int GetCartItemsCount();
}