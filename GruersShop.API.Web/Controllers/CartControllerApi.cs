using GruersShop.Services.Core.Service.Interfaces.Interactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GruersShop.API.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CartControllerApi : ControllerBase
{
    private readonly ICartService _cartService;
    private readonly IOrderService _orderService;

    public CartControllerApi(ICartService cartService,
        IOrderService orderService)
    {
        _cartService = cartService;
        _orderService = orderService;
    }

    [HttpGet]
    public IActionResult GetCart()
    {
        var cart = _cartService.GetCart();
        return Ok(cart);
    }

    [HttpPost]
    public async Task<IActionResult> AddToCart(Guid id, [FromQuery] int quantity = 1)
    {
        await _cartService.AddToCartAsync(id, quantity);
        return Ok(new { success = "Product added to cart!" });
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateQuantity([FromBody] UpdateCartRequest request)
    {
        await _cartService.UpdateQuantityAsync(request.ProductId, request.Quantity);
        return Ok(new { success = "Cart updated successfully!" });
    }

    [HttpDelete("remove/{productId}")]
    public async Task<IActionResult> RemoveFromCart(Guid productId)
    {
        await _cartService.RemoveFromCartAsync(productId);
        return Ok(new { success = "Product removed from cart!" });
    }

    [HttpGet("checkout")]
    public IActionResult GetCheckout()
    {
        var cart = _cartService.GetCart();
        if (!cart.Items.Any())
        {
            return BadRequest(new { error = "Your cart is empty" });
        }
        return Ok(cart);
    }

    [HttpPost("checkout")]
    public async Task<IActionResult> PostCheckout([FromBody] CheckoutRequest request)
    {
        var cart = _cartService.GetCart();
        if (!cart.Items.Any())
        {
            return BadRequest(new { error = "Your cart is empty" });
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        if (userId == null)
        {
            return Unauthorized(new { error = "User not authenticated" });
        }

        try
        {
            var order = await _orderService.CreateOrderFromCartAsync(
                userId,
                cart,
                request.PickupDate,
                request.PickupTime,
                request.SpecialInstructions);

            await _cartService.ClearCartAsync();

            return Ok(new
            {
                success = "Order placed successfully!",
                orderId = order.Id,
                redirectUrl = $"/api/orders/{order.Id}"
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    public class UpdateCartRequest
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class CheckoutRequest
    {
        public string? PickupDate { get; set; }
        public string? PickupTime { get; set; }
        public string? SpecialInstructions { get; set; }
    }
}