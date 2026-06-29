namespace GruersShop.Web.ViewModels.Account;

public class AccountMenuViewModel
{
    public bool IsAuthenticated { get; set; }
    public string? Email { get; set; }
    public int UnreadMessagesCount { get; set; }
}