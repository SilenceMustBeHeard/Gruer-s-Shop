using System;
using System.Collections.Generic;
using System.Text;

namespace GruersShop.Data.Repositories.Interfaces.CRUD
{
    public interface IDeleteRepository<TEntity, TKey> where TEntity : class
    {    // soft delete
        Task<bool> DeleteAsync(TEntity entity);      


        Task<bool> HardDeleteAsync(TEntity entity); 
    }
}
