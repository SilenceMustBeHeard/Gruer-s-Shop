using System.Linq.Expressions;

namespace GruersShop.Data.Repositories.Interfaces.CRUD;

public interface IReadRepository<TEntity, TKey> where TEntity : class
{
    Task<TEntity?> GetByIdAsync(TKey id);

    Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);

    Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);

    Task<int> CountAsync();

    IQueryable<TEntity> Query();
}