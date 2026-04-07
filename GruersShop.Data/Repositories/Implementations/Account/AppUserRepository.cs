using GruersShop.Data.Models;
using GruersShop.Data.Repositories.Implementations.Base;
using GruersShop.Data.Repositories.Interfaces.Account;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GruersShop.Data.Repositories.Implementations.Account
{
    public class AppUserRepository : RepositoryAsync<AppUser, string>
    {
        public AppUserRepository(DbContext context) : base(context)
        {
        }

        // Add AppUser-specific methods here if needed, e.g.:
        public async Task<AppUser?> GetByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}
