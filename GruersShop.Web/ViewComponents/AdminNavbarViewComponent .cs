using Microsoft.AspNetCore.Mvc;

namespace GruersShop.Web.ViewComponents;

public class AdminNavbarViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View();
    }
}