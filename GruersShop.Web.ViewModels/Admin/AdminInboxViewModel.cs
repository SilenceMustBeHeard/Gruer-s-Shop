using GruersShop.Web.ViewModels.Admin.Message;

namespace GruersShop.Web.ViewModels.Admin;

public class AdminInboxViewModel
{
    public List<ContactMessageDetailsViewModel> ContactMessages { get; set; } = new();
}