using GruersShop.Data.Models.Base;
using GruersShop.Services.Core.Service.Admin.Interfaces.Interactions;
using GruersShop.Web.ViewModels.Admin;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GruersShop.Services.Core.Service.Admin.Implementations.Interactions;

public class UserManagementService : IUserManagementService
{
    private readonly UserManager<AppUser> _userManager;

    public UserManagementService(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    // Get all users with the role user except the  admin
    public async Task<IEnumerable<UserManagmentIndexViewModel>> GetUserManagmentBoardDataAsync(Guid adminId)
    {
        var allUsers = await _userManager.Users.ToListAsync();
        var result = new List<UserManagmentIndexViewModel>();

        foreach (var user in allUsers)
        {
            // skip the  admin
            if (user.Id == adminId.ToString())
                continue;

            var roles = await _userManager.GetRolesAsync(user);

            // all roles except "Admin"
            result.Add(new UserManagmentIndexViewModel
            {
                Id = Guid.Parse(user.Id),  // <-- convert back to Guid if needed
                Email = user.Email!,
                Roles = roles,
                LockoutEnd = user.LockoutEnd
            });
        }

        return result;
    }

    // Change user role (starts from "User")
    public async Task<(bool Failed, string ErrorMessage)> ChangeUserRoleAsync(
        ChangeUserRoleViewModel model,
        Guid adminId)
    {
        // find user by Guid - user.Id is already a string, so just use it directly
        var user = await _userManager.FindByIdAsync(model.UserId.ToString());
        if (user == null)
            return (true, "User not found.");

        var roles = await _userManager.GetRolesAsync(user);

        // remove existing roles
        var removeResult = await _userManager.RemoveFromRolesAsync(user, roles);
        if (!removeResult.Succeeded)
            return (true, "Failed to remove existing roles.");

        // add new role
        var addResult = await _userManager.AddToRoleAsync(user, model.NewRole);
        if (!addResult.Succeeded)
            return (true, "Failed to assign new role.");

        // Update security stamp to invalidate existing cookies
        // this forces the user to log in again to get the new role claims,
        // ensuring that the role change takes effect immediately
        var updateStampResult = await _userManager.UpdateSecurityStampAsync(user);

        if (!updateStampResult.Succeeded)
            return (true, "Failed to update security stamp.");

        return (false, string.Empty);
    }

    // finds the user by id and returns the user data with the roles
    public async Task<UserManagmentIndexViewModel> FindUserByIdAsync(string userId)
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

    // Delete user by id

    public async Task<(bool Failed, string ErrorMessage)> DisableUser(string userId)
    {
        var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            return (true, "User not found.");

        // sets the value to the maximum possible date, effectively locking the user out indefinitely
        // it can also be used for kick/ban user for set amount of time by setting the value to DateTimeOffset.UtcNow.AddMinutes(30) for example

        user.LockoutEnd = DateTimeOffset.MaxValue;

        await _userManager.UpdateAsync(user);

        return (false, string.Empty);
    }
}