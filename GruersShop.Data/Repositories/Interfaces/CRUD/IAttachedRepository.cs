namespace GruersShop.Data.Repositories.Interfaces.CRUD
{
    public interface IAttachedRepository<TEntity, TKey> where TEntity : class
    {
        IQueryable<TEntity> GetAllAttachedAsync();
    }
}