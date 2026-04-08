using GruersShop.Web.ViewModels.Account.Messages;
using System.Collections.Generic;

namespace GruersShop.Web.ViewModels.Admin
{
    public class AdminInboxViewModel
    {
 
        public List<ContactMessageDetailsViewModel> ContactMessages { get; set; } = new();
    }
}