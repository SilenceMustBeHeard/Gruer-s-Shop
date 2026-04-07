using System;
using System.Collections.Generic;
using System.Text;

namespace GruersShop.Services.Core.Service.Interfaces.Account;

public interface IAccountService
{

    Task<(bool Success, string[] Errors)> RegisterAsync(RegisterViewModel model);

    Task<bool> LoginAsync(LoginViewModel model);

    Task LogoutAsync();

    Task<bool> ForgotPasswordAsync(string email, string resetLink);
    Task<(bool Success, string[] Errors)> ResetPasswordAsync(ResetPasswordViewModel model);
}