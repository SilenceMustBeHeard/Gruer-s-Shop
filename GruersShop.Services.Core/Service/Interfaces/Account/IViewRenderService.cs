using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace GruersShop.Services.Core.Service.Interfaces.Account;

public interface IViewRenderService
{
    Task<string> RenderToStringAsync(string viewName, object model, ViewDataDictionary? viewData = null);
}