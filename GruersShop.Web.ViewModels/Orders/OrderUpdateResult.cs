using GruersShop.Data.Models.Interactions;

using System;
using System.Collections.Generic;

using System.Text;

namespace GruersShop.Web.ViewModels.Orders;

public class OrderUpdateResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public Order? Order { get; set; }
}