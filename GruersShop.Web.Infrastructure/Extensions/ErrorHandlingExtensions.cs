using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;

namespace GruersShop.Web.Infrastructure.Extensions;

public static class ErrorHandlingExtensions
{
    public static IApplicationBuilder UseCustomErrorHandling(this IApplicationBuilder app)
    {
        return app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                context.Response.ContentType = "text/html";

                var exceptionHandlerPathFeature =
                    context.Features.Get<IExceptionHandlerPathFeature>();

                var exception = exceptionHandlerPathFeature?.Error;



                // Redirect to custom error page
                context.Response.Redirect("/Error/500");
            });
        });
    }
}