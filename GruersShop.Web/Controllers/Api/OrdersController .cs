using GruersShop.Services.Core.Service.Interfaces.Interactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GruersShop.Web.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderManagementService _orderManagementService;

        public OrdersController(IOrderManagementService orderManagementService)
        {
            _orderManagementService = orderManagementService;
        }

        [HttpGet("recent-updates")]
        public async Task<IActionResult> GetRecentUpdates()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Ok(new { hasUpdates = false });

            var fiveMinutesAgo = DateTime.UtcNow.AddMinutes(-5);
            var hasRecentUpdates = await _orderManagementService.HasRecentUpdatesAsync(userId, fiveMinutesAgo);

            return Ok(new { hasUpdates = hasRecentUpdates });
        }
    }
}