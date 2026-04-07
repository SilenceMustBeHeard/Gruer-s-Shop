using System;
using System.Collections.Generic;
using System.Text;

namespace GruersShop.Data.Repositories.Interfaces.CRUD
{
    public interface IAttachedRepository<TEntity, TKey> where TEntity : class
    {
        IQueryable<TEntity> GetAllAttachedAsync();
    }
}
