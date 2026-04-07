using System;
using System.Collections.Generic;
using System.Text;

namespace GruersShop.Data.Repositories.Interfaces.CRUD
{
    public interface IWriteRepository<TEntity, TKey> where TEntity : class
    {
        Task AddAsync(TEntity entity);
        Task AddRangeAsync(IEnumerable<TEntity> entities);
        Task<bool> UpdateAsync(TEntity entity);
        Task SaveChangesAsync();
    }
}
