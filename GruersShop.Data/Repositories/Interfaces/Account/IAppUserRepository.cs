using GruersShop.Data.Models.Base;
using GruersShop.Data.Repositories.Interfaces.CRUD;

namespace GruersShop.Data.Repositories.Interfaces.Account;

public interface IAppUserRepository
    : IFullRepositoryAsync<AppUser, string>
{
}