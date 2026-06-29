using Microsoft.AspNetCore.Mvc;

namespace GruersShop.API.Web.Controllers.Areas.User.Account;

[Route("api/[controller]")]
[ApiController]
public class ErrorControllerApi : ControllerBase
{
    [HttpGet("{statusCode}")]
    public IActionResult Index(int statusCode)
    {
        return statusCode switch
        {
            400 => BadRequest(new { error = "Bad Request", statusCode = 400 }),
            401 => Unauthorized(new { error = "Unauthorized", statusCode = 401 }),
            403 => StatusCode(403, new { error = "Forbidden", statusCode = 403 }),
            404 => NotFound(new { error = "Not Found", statusCode = 404 }),
            501 => StatusCode(501, new { error = "Not Implemented", statusCode = 501 }),
            _ => StatusCode(500, new { error = "Internal Server Error", statusCode = 500 })
        };
    }

    public static IActionResult ReturnError(int statusCode, string message)
    {
        return new ObjectResult(new { error = message, statusCode })
        {
            StatusCode = statusCode
        };
    }
}