using Microsoft.AspNetCore.Mvc;

namespace GruersShop.Web.Controllers.Account;

public class ErrorController : Controller
{
    // Handles errors (not found or not developed features)
    [Route("Error/NotImplemented")]
    [Route("Error/NotImplemented/{feature}")]
    public IActionResult NotImplemented(string? feature)
    {
        ViewBag.FeatureName = feature ?? "This feature";
        ViewBag.ErrorCode = 501;
        Response.StatusCode = 501;
        return View();
    }

    // Handles errors (forbidden access)
    [Route("Error/NotAllowed")]
    [Route("Error/AccessDenied")]
    [Route("Error/403")]
    public IActionResult NotAllowed()
    {
        ViewBag.ErrorCode = 403;
        Response.StatusCode = 403;
        return View();
    }

    // Handles not found errors
    [Route("Error/NotFound")]
    [Route("Error/404")]
    public IActionResult NotFound()
    {
        ViewBag.ErrorCode = 404;
        Response.StatusCode = 404;
        return View();
    }

    // Handles server errors
    [Route("Error/ServerError")]
    [Route("Error/500")]
    public IActionResult ServerError()
    {
        ViewBag.ErrorCode = 500;
        Response.StatusCode = 500;
        return View();
    }

    // Handles bad requests
    [Route("Error/BadRequest")]
    [Route("Error/400")]
    public IActionResult BadRequest()
    {
        ViewBag.ErrorCode = 400;
        Response.StatusCode = 400;
        return View();
    }

    // General error handler with status code
    [Route("Error/{statusCode?}")]
    public IActionResult Index(int? statusCode)
    {
        ViewBag.ErrorCode = statusCode ?? 500;
        Response.StatusCode = statusCode ?? 500;

        return statusCode switch
        {
            404 => View("NotFound"),
            403 => View("NotAllowed"),
            400 => View("BadRequest"),
            501 => View("NotImplemented"),
            500 => View("ServerError"),
            _ => View("ServerError")
        };
    }
}