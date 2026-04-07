using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace GruersShop.Data.Repositories.Interfaces.CRUD
{
    public interface IFindRepository<TEntity, TKey> where TEntity : class
    {
        Task<TEntity?> FindByConditionAsync(Expression<Func<TEntity, bool>> predicate);
    }
}
