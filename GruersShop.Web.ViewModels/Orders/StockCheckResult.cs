namespace GruersShop.Web.ViewModels.Orders;

public class StockCheckResult
{
    public bool IsAvailable { get; set; }
    public List<StockIssue> Issues { get; set; } = new();
}