using System;
using System.Collections.Generic;
using System.Text;

namespace GruersShop.Services.Core.Service.Interfaces.Account;

public interface IManagerService
{
    Task<bool> IsUserManagerAsync(string userId);
}