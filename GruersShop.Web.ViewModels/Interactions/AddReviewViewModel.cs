namespace GruersShop.Web.ViewModels.Interactions;

public class AddReviewViewModel
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public int Rating { get; set; }
    public string? Comment { get; set; }
}