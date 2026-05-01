using System;
using System.Collections.Generic;
using System.Text;

namespace GruersShop.Web.ViewModels.Orders;


public class OrderItemViewModel
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public string? ProductImageUrl { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }

    public string FormattedUnitPrice => UnitPrice.ToString("C");
    public string FormattedTotal => TotalPrice.ToString("C");
}