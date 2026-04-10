
using GruersShop.Data.Models.Catalog;
using GruersShop.Data.Repositories.Interfaces.CRUD;

namespace GruersShop.Data.Repositories.Interfaces.Bakery;

public interface ICatalogRepository : IFullRepositoryAsync<Catalog, Guid>
{
    Task<IEnumerable<Catalog>> GetAllActiveAsync();
    Task<Catalog?> GetByIdWithDetailsAsync(Guid id);
    Catalog? GetByName(string name);
    Task<IEnumerable<Catalog>> GetAllForAdminAsync();
    Task ToggleCatalogStatusAsync(Catalog catalog);
    Task<Catalog?> GetByIdIncludingDeletedAsync(Guid id);
}