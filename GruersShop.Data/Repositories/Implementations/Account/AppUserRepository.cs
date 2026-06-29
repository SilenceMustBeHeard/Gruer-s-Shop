using GruersShop.Data.Models.Base;
using GruersShop.Data.Repositories.Implementations.Base;
using GruersShop.Data.Repositories.Interfaces.Account;
using Microsoft.EntityFrameworkCore;

namespace GruersShop.Data.Repositories.Implementations.Account;

public class AppUserRepository : RepositoryAsync<AppUser, string>, IAppUserRepository
{
    public AppUserRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<AppUser?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
    }
}