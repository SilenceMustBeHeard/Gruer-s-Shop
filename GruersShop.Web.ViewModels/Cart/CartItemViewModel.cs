using System;
using System.Collections.Generic;
using System.Text;

namespace GruersShop.Web.ViewModels.Cart;

public class CartItemViewModel
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public string ImageUrl { get; set; } = null!;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public decimal Subtotal => Price * Quantity;
}