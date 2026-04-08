using GruersShop.Data.Repositories.Interfaces.CRUD;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Text;

namespace GruersShop.Data.Repositories.Implementations.Base
{
    public class RepositoryAsync<TEntity, TKey> : IFullRepositoryAsync<TEntity, TKey>
      where TEntity : class
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        public RepositoryAsync(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        // IReadRepository
        public async Task<TEntity?> GetByIdAsync(TKey id)
            => await _dbSet.FindAsync(id);

        public async Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
            => await _dbSet.SingleOrDefaultAsync(predicate);

        public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
            => await _dbSet.FirstOrDefaultAsync(predicate);

        public async Task<int> CountAsync()
            => await _dbSet.CountAsync();

        public IQueryable<TEntity> Query()
            => _dbSet;

        // IWriteRepository
        public async Task AddAsync(TEntity entity)
            => await _dbSet.AddAsync(entity);

        public async Task AddRangeAsync(IEnumerable<TEntity> entities)
            => await _dbSet.AddRangeAsync(entities);

        public Task<bool> UpdateAsync(TEntity entity)
        {
            _dbSet.Update(entity);
            return Task.FromResult(true);
        }

        public async Task SaveChangesAsync()
            => await _context.SaveChangesAsync();

        // IDeleteRepository
        public Task<bool> DeleteAsync(TEntity entity)
        {
            _dbSet.Remove(entity);
            return Task.FromResult(true);
        }

        public Task<bool> HardDeleteAsync(TEntity entity)
        {
            _dbSet.Remove(entity);
            return Task.FromResult(true);
        }

        // ISoftDeleteQueryRepository
        public IQueryable<TEntity> GetAllIncludingDeleted()
            => _dbSet.IgnoreQueryFilters();

        public Task<bool> ToggleStatusAsync(TEntity entity)
        {
            // Assuming entity has IsActive property via interface
            var property = entity.GetType().GetProperty("IsActive");
            if (property != null)
            {
                property.SetValue(entity, !(bool)property.GetValue(entity));
                _dbSet.Update(entity);
            }
            return Task.FromResult(true);
        }

        // IAttachedRepository
        public IQueryable<TEntity> GetAllAttachedAsync()
            => _dbSet.AsNoTracking();

        // ICategoryRepository
        public async Task<IEnumerable<TEntity>> GetCategoriesAsync()
            => await _dbSet.ToListAsync();

        // IFindRepository
        public async Task<TEntity?> FindByConditionAsync(Expression<Func<TEntity, bool>> predicate)
            => await _dbSet.FirstOrDefaultAsync(predicate);

   
    }
}
