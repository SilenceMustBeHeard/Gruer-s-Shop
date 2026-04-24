using System;
using System.Collections.Generic;
using System.Text;

namespace GruersShop.Web.ViewModels.Orders;

public class StockCheckResult
{
    public bool IsAvailable { get; set; }
    public List<StockIssue> Issues { get; set; } = new();
}
