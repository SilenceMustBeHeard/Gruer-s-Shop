using System;
using System.Collections.Generic;
using System.Text;

namespace GruersShop.Web.ViewModels.Cart;

public class CartViewModel
{
    public List<CartItemViewModel> Items { get; set; } = new();
    public decimal TotalAmount => Items.Sum(i => i.Subtotal);
    public int TotalItems => Items.Sum(i => i.Quantity);
}