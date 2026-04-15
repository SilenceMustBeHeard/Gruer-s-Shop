using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ErrorController : Controller
{
    public IActionResult NotImplemented(string? feature)
    {
        ViewBag.FeatureName = feature ?? "This feature";
        ViewBag.ErrorCode = 501;
        Response.StatusCode = 501;
        return View();
    }

    public IActionResult NotAllowed()
    {
        ViewBag.ErrorCode = 403;
        Response.StatusCode = 403;
        return View();
    }

    public IActionResult NotFound()
    {
        ViewBag.ErrorCode = 404;
        Response.StatusCode = 404;
        return View();
    }

    public IActionResult ServerError()
    {
        ViewBag.ErrorCode = 500;
        Response.StatusCode = 500;
        return View();
    }

    public IActionResult BadRequest()
    {
        ViewBag.ErrorCode = 400;
        Response.StatusCode = 400;
        return View();
    }
}