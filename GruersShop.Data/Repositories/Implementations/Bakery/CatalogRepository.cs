using GruersShop.Data.Models.Catalog;
using GruersShop.Data.Repositories.Implementations.Base;
using GruersShop.Data.Repositories.Interfaces.Bakery;

using Microsoft.EntityFrameworkCore;

namespace GruersShop.Data.Repositories.Implementations.Bakery;

public class CatalogRepository : RepositoryAsync<Catalog, Guid>, ICatalogRepository
{
    private readonly AppDbContext _context;

    public CatalogRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Catalog>> GetAllActiveAsync()
    {
        return await _dbSet
            .Where(c => !c.IsDeleted)
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync();
    }

    public async Task<Catalog?> GetByIdWithDetailsAsync(Guid id)
    {
        return await _dbSet
            .Include(c => c.Categories)
                .ThenInclude(cat => cat.Products)
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
    }

    public Catalog? GetByName(string name)
    {
        return _dbSet
            .FirstOrDefault(c => c.Name == name && !c.IsDeleted);
    }

    public async Task<IEnumerable<Catalog>> GetAllForAdminAsync()
    {
        return await _dbSet
            .IgnoreQueryFilters()
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync();
    }

    public async Task ToggleCatalogStatusAsync(Catalog catalog)
    {
        catalog.IsDeleted = !catalog.IsDeleted;
        catalog.UpdatedAt = DateTime.UtcNow;
        _dbSet.Update(catalog);
        await _context.SaveChangesAsync();
    }

    public async Task<Catalog?> GetByIdIncludingDeletedAsync(Guid id)
    {
        return await _dbSet
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(c => c.Id == id);
    }


}