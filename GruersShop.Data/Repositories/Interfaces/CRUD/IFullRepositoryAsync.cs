using System;
using System.Collections.Generic;
using System.Text;

namespace GruersShop.Data.Repositories.Interfaces.CRUD;


//  combines all the CRUD operations into a single interface for convenience
public interface IFullRepositoryAsync<TEntity, TKey> :
  IReadRepository<TEntity, TKey>,
  IWriteRepository<TEntity, TKey>,
  IDeleteRepository<TEntity, TKey>,
  ISoftDeleteQueryRepository<TEntity, TKey>,
  IAttachedRepository<TEntity, TKey>,
  ICategoryRepository<TEntity, TKey>,
  IFindRepository<TEntity, TKey>
  where TEntity : class
{
    
}
