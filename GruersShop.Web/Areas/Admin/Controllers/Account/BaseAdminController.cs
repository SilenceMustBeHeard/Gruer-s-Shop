using GruersShop.Data.Models.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GruersShop.Web.Areas.Admin.Controllers.Account;


[Area("Admin")]
[Authorize(Roles = "Admin")]
public abstract class BaseAdminController : Controller
{

    protected BaseAdminController(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }
    
    protected void SetToastMessage(string message, string type = "success")
    {
        Response.Headers.Add("X-Toast-Message", message);
        Response.Headers.Add("X-Toast-Type", type);
    }

  

    protected bool IsUserAdmin() => User.IsInRole("Admin");

    protected bool IsUserAuthenticated() => User.Identity?.IsAuthenticated ?? false;



    private readonly UserManager<AppUser> _userManager;

    protected Guid GetUserId() => Guid.Parse(_userManager.GetUserId(User));
    protected async Task<AppUser?> GetCurrentUserAsync()
    {
        return await _userManager.GetUserAsync(User);
    }
}