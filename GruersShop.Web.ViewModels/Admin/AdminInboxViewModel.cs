using GruersShop.Web.ViewModels.Account.Messages;
using GruersShop.Web.ViewModels.Admin.Message;
using System.Collections.Generic;

namespace GruersShop.Web.ViewModels.Admin
{
    public class AdminInboxViewModel
    {
 
        public List<ContactMessageDetailsViewModel> ContactMessages { get; set; } = new();
    }
}