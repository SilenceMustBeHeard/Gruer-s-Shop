using GruersShop.Data.Models.Catalog;
using GruersShop.Data.Repositories.Implementations.Base;
using GruersShop.Data.Repositories.Interfaces.Bakery;
using Microsoft.EntityFrameworkCore;

namespace GruersShop.Data.Repositories.Implementations.Bakery
{
    public class CategoryRepository : RepositoryAsync<Category, Guid>, ICategoryRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Category?> GetByIdIncludingDeletedAsync(Guid id)
        {
            return await _dbSet
                .IgnoreQueryFilters()
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Category>> GetAllActiveAsync()
        {
            return await _dbSet
                .Where(c => !c.IsDeleted)
                .Include(c => c.Catalog)
                .OrderBy(c => c.DisplayOrder)
                .ToListAsync();
        }
        public async Task<Catalog> GetDefaultCatalogAsync()
        {
            var catalog = await _context.Catalogs.FirstOrDefaultAsync();
            if (catalog == null)
            {
              
                catalog = new Catalog
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Name = "Gruer's Signature Collection",
                    Description = "Our most beloved baked goods, crafted with love",
                    DisplayOrder = 1,
                    CreatedAt = DateTime.UtcNow
                };
                await _context.Catalogs.AddAsync(catalog);
                await _context.SaveChangesAsync();
            }
            return catalog;
        }

        public async Task<IEnumerable<Category>> GetAllForAdminAsync()
        {
            return await _dbSet
                .IgnoreQueryFilters()
                .Include(c => c.Catalog)
                .OrderBy(c => c.DisplayOrder)
                .ToListAsync();
        }

        public async Task ToggleCategoryStatusAsync(Category category)
        {
           
            category.IsDeleted = !category.IsDeleted;
            category.UpdatedAt = DateTime.UtcNow;
            _dbSet.Update(category);
            await _context.SaveChangesAsync();
        }

        public Category? GetByName(string name)
        {
            return _dbSet
                .FirstOrDefault(c => c.Name == name && !c.IsDeleted);
        }
    }
}