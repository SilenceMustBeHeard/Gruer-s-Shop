using GruersShop.Data.Models.Base;
using GruersShop.Data.Models.Products;
using System.ComponentModel.DataAnnotations;

namespace GruersShop.Data.Models.Interactions;

public class OrderItem : BaseDeletableEntity
{
    public Guid OrderId { get; set; }
    public virtual Order Order { get; set; } = null!;

    public Guid ProductId { get; set; }
    public virtual Product Product { get; set; } = null!;

    [Range(1, 99)]
    public int Quantity { get; set; }

    [Range(0.01, 40.00)]
    public decimal UnitPrice { get; set; }

    public decimal Subtotal => Quantity * UnitPrice;
}