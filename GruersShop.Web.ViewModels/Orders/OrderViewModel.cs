using System;
using System.Collections.Generic;
using System.Linq;
using GruersShop.Data.Common.Enums;

namespace GruersShop.Web.ViewModels.Orders;

public class OrderViewModel
{
    public Guid Id { get; set; }

    public string UserId { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string UserEmail { get; set; } = null!;
    public string? UserFullName { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; }

    public DateTime? OrderDate { get; set; }
    public DateTime? PickupDate { get; set; }
    public string? PickupTime { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public string? SpecialInstructions { get; set; }
    public string? CancellationReason { get; set; }

    public bool IsDeleted { get; set; }

    public List<OrderItemViewModel> Items { get; set; } = new();

    public string OrderNumber => Id.ToString("N").Substring(0, 8).ToUpper();
    public string FormattedTotal => TotalAmount.ToString("C");
    public string FormattedOrderDate => OrderDate?.ToString("dd MMM yyyy, HH:mm") ?? "N/A";

    public string FormattedPickupDisplay => PickupDate.HasValue
        ? $"{PickupDate:dd MMM yyyy} at {PickupTime}"
        : "To be determined";

    public bool CanCancel => new[]
    {
        OrderStatus.Pending,
        OrderStatus.Approved,
        OrderStatus.Confirmed,
        OrderStatus.Baking
    }.Contains(Status) && !IsDeleted;

    public int TotalItemCount => Items.Sum(i => i.Quantity);
    public bool HasSpecialInstructions => !string.IsNullOrWhiteSpace(SpecialInstructions);

    public string StatusColor => Status switch
    {
        OrderStatus.Pending => "warning",
        OrderStatus.Approved => "info",
        OrderStatus.Confirmed => "primary",
        OrderStatus.Baking => "secondary",
        OrderStatus.ReadyForPickup => "success",
        OrderStatus.Completed => "dark",
        OrderStatus.Cancelled => "danger",
        OrderStatus.Rejected => "danger",
        _ => "light"
    };
}