using System.Linq.Expressions;

namespace GruersShop.Data.Repositories.Interfaces.CRUD
{
    public interface IFindRepository<TEntity, TKey> where TEntity : class
    {
        Task<TEntity?> FindByConditionAsync(Expression<Func<TEntity, bool>> predicate);
    }
}