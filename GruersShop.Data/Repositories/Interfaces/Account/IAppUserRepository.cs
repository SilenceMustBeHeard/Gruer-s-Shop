using GruersShop.Data.Models.Base;
using GruersShop.Data.Repositories.Interfaces.CRUD;
using System;
using System.Collections.Generic;
using System.Text;

namespace GruersShop.Data.Repositories.Interfaces.Account
{
    public interface IAppUserRepository 
        : IFullRepositoryAsync<AppUser, string>
    {
    }
}
