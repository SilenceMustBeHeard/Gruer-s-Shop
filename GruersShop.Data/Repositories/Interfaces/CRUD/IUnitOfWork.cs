using System;
using System.Collections.Generic;
using System.Text;

namespace GruersShop.Data.Repositories.Interfaces.CRUD;

public interface IUnitOfWork : IDisposable
{
    IFullRepositoryAsync<TEntity, TKey> Repository<TEntity, TKey>() where TEntity : class;
    Task<int> CommitAsync();
    Task RollbackAsync();
    Task BeginTransactionAsync();
}
