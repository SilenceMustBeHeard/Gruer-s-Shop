using System;
using System.Collections.Generic;
using System.Text;

namespace GruersShop.Web.ViewModels.Orders;

public class StockIssue
{
    public string ProductName { get; set; } = string.Empty;
    public int RequestedQuantity { get; set; }
    public int AvailableStock { get; set; }
    public int Shortage => RequestedQuantity - AvailableStock;
}