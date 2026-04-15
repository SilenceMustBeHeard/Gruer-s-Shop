using GruersShop.Web.ViewModels.Admin;
using System;
using System.Collections.Generic;
using System.Text;

namespace GruersShop.Services.Core.Service.Admin.Interfaces.Interactions;

public interface IUserManagementService
{
    Task<IEnumerable<UserManagmentIndexViewModel>> GetUserManagmentBoardDataAsync(Guid userId);
    Task<UserManagmentIndexViewModel> FindUserByIdAsync(string userId);


    Task<(bool Failed, string ErrorMessage)> DisableUser(string userId);


    Task<(bool Failed, string ErrorMessage)> ChangeUserRoleAsync(
        ChangeUserRoleViewModel model,
        Guid adminId);
}
