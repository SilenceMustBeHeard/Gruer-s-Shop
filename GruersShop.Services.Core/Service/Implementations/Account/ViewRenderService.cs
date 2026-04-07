using GruersShop.Services.Core.Service.Interfaces.Account;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Text;

namespace GruersShop.Services.Core.Service.Implementations.Account;

     //This service is used to render Razor views to strings
    // which can be useful for generating email content or other dynamic HTML content.
    public class ViewRenderService : IViewRenderService
{
    private readonly IRazorViewEngine _razorViewEngine;
    private readonly ITempDataProvider _tempDataProvider;
    private readonly IServiceProvider _serviceProvider;

    public ViewRenderService(
        IRazorViewEngine razorViewEngine,
        ITempDataProvider tempDataProvider,
        IServiceProvider serviceProvider)
    {
        _razorViewEngine = razorViewEngine;
        _tempDataProvider = tempDataProvider;
        _serviceProvider = serviceProvider;
    }


    public async Task<string> RenderToStringAsync(string viewName, object model, ViewDataDictionary? viewData = null)
    {

        // Create a fake ActionContext to render the view outside of a controller
        var httpContext = new DefaultHttpContext { RequestServices = _serviceProvider };

        // The ActionContext is required by the Razor view engine to find and render the view
        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());

        using var stringWriter = new StringWriter();


        // Find the view using the Razor view engine
        var viewResult = _razorViewEngine.FindView(actionContext, viewName, false);

        if (viewResult.View == null)
        {
            throw new ArgumentNullException($"{viewName} view not found.");
        }


        // Create a ViewDataDictionary to pass the model and any additional data to the view
        var viewDictionary = viewData ?? new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());
        viewDictionary.Model = model;


        // Create a TempDataDictionary to pass temporary data to the view
        var tempData = new TempDataDictionary(httpContext, _tempDataProvider);
        var viewContext = new ViewContext(
            actionContext,
            viewResult.View,
            viewDictionary,
            tempData,
            stringWriter,
            new HtmlHelperOptions()
        );

        await viewResult.View.RenderAsync(viewContext);
        return stringWriter.ToString();
    }
}