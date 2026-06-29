using GruersShop.Data.Models.Interactions;

namespace GruersShop.Web.ViewModels.Orders;

public class OrderUpdateResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public Order? Order { get; set; }
}