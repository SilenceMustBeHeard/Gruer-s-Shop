using System;
using System.Collections.Generic;
using System.Text;

namespace GruersShop.Web.ViewModels.Account.Profile;

public class NavbarButtonsViewModel
{
    public bool IsLoggedIn { get; set; }
    public bool IsAdmin { get; set; }
    public bool IsManager { get; set; }
    public bool IsUser { get; set; }
    public int PendingOrdersCount { get; set; }
    public int UnreadMessagesCount { get; set; }


    //  used to highlight the current area in the navbar
    public string? CurrentArea { get; set; }


}
