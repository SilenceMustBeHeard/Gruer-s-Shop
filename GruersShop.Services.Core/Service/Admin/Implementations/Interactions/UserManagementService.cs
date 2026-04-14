using GruersShop.Data.Models.Base;
using GruersShop.Services.Core.Service.Admin.Interfaces.Interactions;
using GruersShop.Web.ViewModels.Admin;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GruersShop.Services.Core.Service.Admin.Implementations.Interactions;

public class UserManagementService : IUserManagementService
{
    private readonly UserManager<AppUser> _userManager;

    public UserManagementService(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IEnumerable<UserManagmentIndexViewModel>> GetUserManagmentBoardDataAsync(Guid adminId)
    {
        var allUsers = await _userManager.Users.ToListAsync();
        var result = new List<UserManagmentIndexViewModel>();

        foreach (var user in allUsers)
        {
            // skip the current admin
            if (user.Id == adminId.ToString())
                continue;

            var roles = await _userManager.GetRolesAsync(user);

            result.Add(new UserManagmentIndexViewModel
            {
                Id = Guid.Parse(user.Id),
                Email = user.Email!,
                Roles = roles,
                LockoutEnd = user.LockoutEnd
            });
        }

        return result;
    }

    public async Task<(bool Failed, string ErrorMessage)> ChangeUserRoleAsync(
        ChangeUserRoleViewModel model,
        Guid adminId)
    {
        var user = await _userManager.FindByIdAsync(model.UserId.ToString());
        if (user == null)
            return (true, "User not found.");

        var roles = await _userManager.GetRolesAsync(user);

        var removeResult = await _userManager.RemoveFromRolesAsync(user, roles);
        if (!removeResult.Succeeded)
            return (true, "Failed to remove existing roles.");

        var addResult = await _userManager.AddToRoleAsync(user, model.NewRole);
        if (!addResult.Succeeded)
            return (true, "Failed to assign new role.");

        await _userManager.UpdateSecurityStampAsync(user);

        return (false, string.Empty);
    }

    public async Task<UserManagmentIndexViewModel?> FindUserByIdAsync(string userId)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            return null;

        var roles = await _userManager.GetRolesAsync(user);

        return new UserManagmentIndexViewModel
        {
            Id = Guid.Parse(user.Id),
            Email = user.Email!,
            Roles = roles,
            LockoutEnd = user.LockoutEnd
        };
    }

    public async Task<(bool Failed, string ErrorMessage)> DisableUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return (true, "User not found.");

        // Toggle ban/unban
        if (user.LockoutEnd != null && user.LockoutEnd > DateTimeOffset.UtcNow)
        {
            user.LockoutEnd = null;  // Unban
        }
        else
        {
            user.LockoutEnd = DateTimeOffset.MaxValue;  // Ban
        }

        await _userManager.UpdateAsync(user);

        return (false, string.Empty);
    }
}