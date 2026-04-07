using System;
using System.Collections.Generic;
using System.Text;

namespace GruersShop.Data.Repositories.Interfaces.CRUD
{
    public interface ICategoryRepository<TEntity, TKey> where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetCategoriesAsync();
    }
}
