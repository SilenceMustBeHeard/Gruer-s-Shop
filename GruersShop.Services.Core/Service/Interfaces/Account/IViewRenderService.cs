using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Text;

namespace GruersShop.Services.Core.Service.Interfaces.Account;

public interface IViewRenderService
{
    Task<string> RenderToStringAsync(string viewName, object model, ViewDataDictionary? viewData = null);
}
