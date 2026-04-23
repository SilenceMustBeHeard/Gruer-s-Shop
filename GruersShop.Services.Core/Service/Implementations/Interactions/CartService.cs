using GruersShop.Services.Core.Service.Interfaces.Bakery;
using GruersShop.Services.Core.Service.Interfaces.Interactions;
using GruersShop.Web.ViewModels.Cart;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace GruersShop.Services.Core.Service.Implementations.Interactions;

public class CartService : ICartService
{
    private const string CartSessionKey = "ShoppingCart";
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ICatalogClientService _catalogService;
    private CartViewModel? _cart;

    public CartService(IHttpContextAccessor httpContextAccessor, ICatalogClientService catalogService)
    {
        _httpContextAccessor = httpContextAccessor;
        _catalogService = catalogService;
    }

    public ISession Session => _httpContextAccessor.HttpContext!.Session;

    public CartViewModel GetCart()
    {
        if (_cart != null) return _cart;

        var cartJson = Session.GetString(CartSessionKey);
        if (string.IsNullOrEmpty(cartJson))
        {
            _cart = new CartViewModel();
        }
        else
        {
            _cart = JsonConvert.DeserializeObject<CartViewModel>(cartJson) ?? new CartViewModel();
        }

        return _cart;
    }

    public void SaveCart(CartViewModel cart)
    {
        _cart = cart;
        Session.SetString(CartSessionKey, JsonConvert.SerializeObject(cart));
    }

    public async Task AddToCartAsync(Guid productId, int quantity = 1)
    {
        var cart = GetCart();
        var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);

        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
        }
        else
        {
            var product = await _catalogService.GetProductDetailsViewModelAsync(productId, null);
            if (product == null) throw new Exception("Product not found");

            cart.Items.Add(new CartItemViewModel
            {
                ProductId = productId,
                ProductName = product.Product.Name,
                ImageUrl = product.Product.ImageUrl,
                Price = product.Product.Price,
                Quantity = quantity
            });
        }

        SaveCart(cart);
    }

    public async Task RemoveFromCartAsync(Guid productId)
    {
        var cart = GetCart();
        var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
        if (item != null)
        {
            cart.Items.Remove(item);
            SaveCart(cart);
        }
    }

    public async Task UpdateQuantityAsync(Guid productId, int quantity)
    {
        if (quantity <= 0)
        {
            await RemoveFromCartAsync(productId);
            return;
        }

        var cart = GetCart();
        var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
        if (item != null)
        {
            item.Quantity = quantity;
            SaveCart(cart);
        }
    }

    public async Task ClearCartAsync()
    {
         await Task.Run(() => Session.Remove(CartSessionKey));
        _cart = null;
    }

    public int GetCartItemsCount()
    {
        return GetCart().TotalItems;
    }
}