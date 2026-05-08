using GruersShop.Data.Models.Base;
using GruersShop.Services.Core.Service.Admin.Interfaces.Interactions;
using GruersShop.Web.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GruersShop.API.Web.Controllers.Areas.Admin.Interactions;

[Route("api/admin/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class UserManagementControllerApi : ControllerBase
{
    private readonly IUserManagementService _userService;
    private readonly UserManager<AppUser> _userManager;

    public UserManagementControllerApi(
        IUserManagementService userService,
        UserManager<AppUser> userManager)
    {
        _userService = userService;
        _userManager = userManager;
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
       
        var currentUserId = GetCurrentUserId();
        if (currentUserId == null)
        {
            return Unauthorized(new { error = "Unable to identify current user" });
        }

        var allUsers = await _userService.GetUserManagmentBoardDataAsync(currentUserId.Value);

        if (allUsers == null || !allUsers.Any())
        {
            return Ok(new { users = new List<object>(), message = "No users found" });
        }

        return Ok(allUsers);
    }

    [HttpPost("assign-role")]
    public async Task<IActionResult> AssignRole([FromBody] ChangeUserRoleViewModel model)
    {
        if (model == null)
        {
            return BadRequest(new { error = "Invalid request body" });
        }

        if (string.IsNullOrWhiteSpace(model.NewRole))
        {
            return BadRequest(new { error = "Please select a valid role." });
        }

        var currentUserId = GetCurrentUserId();
        if (currentUserId == null)
        {
            return Unauthorized(new { error = "Unable to identify current user" });
        }

        var result = await _userService.ChangeUserRoleAsync(model, currentUserId.Value);

        if (result.Failed)
        {
            return BadRequest(new { error = result.ErrorMessage });
        }

        return Ok(new
        {
            success = true,
            message = "User role changed successfully.",
            userId = model.UserId,
            newRole = model.NewRole
        });
    }

    [HttpPost("disable/{userId}")]
    public async Task<IActionResult> DisableUser(string userId)
    {
       
        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest(new { error = "User ID is required" });
        }

        var currentUserId = GetCurrentUserId();
        if (currentUserId == null)
        {
            return Unauthorized(new { error = "Unable to identify current user" });
        }

        if (userId == currentUserId.Value.ToString())
        {
            return BadRequest(new { error = "You cannot disable your own account" });
        }

        var result = await _userService.DisableUser(userId);

        if (result.Failed)
        {
            return BadRequest(new { error = result.ErrorMessage });
        }

        return Ok(new
        {
            success = true,
            message = "User disabled successfully.",
            userId = userId
        });
    }

    //[HttpPost("enable/{userId}")]
    //public async Task<IActionResult> EnableUser(string userId)
    //{
    //    if (string.IsNullOrEmpty(userId))
    //       {
    //       return BadRequest(new { error = "User ID is required" });
    //       }

    //    var currentUserId = GetCurrentUserId();
    //    if (currentUserId == null)
    //      {
    //      return Unauthorized(new { error = "Unable to identify current user" });
    //      }

    //    var result = await _userService.EnableUser(userId);

    //    if (result.Failed)
    //     {
    //     return BadRequest(new { error = result.ErrorMessage });
    //     }

    //    return Ok(new
    //    {
    //        success = true,
    //        message = "User enabled successfully.",
    //        userId = userId
    //    });
    //}

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userIdClaim))
        {
            return null;
        }

        if (Guid.TryParse(userIdClaim, out var userId))
        {
            return userId;
        }
        return null;
    }
}