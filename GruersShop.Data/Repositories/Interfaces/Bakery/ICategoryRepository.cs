using GruersShop.Data.Models.Catalog;
using GruersShop.Data.Repositories.Interfaces.CRUD;
using System;
using System.Collections.Generic;
using System.Text;

namespace GruersShop.Data.Repositories.Interfaces.Bakery;

public interface ICategoryRepository : IFullRepositoryAsync<Category, Guid>
{
    Task<Catalog> GetDefaultCatalogAsync();
    Task<Category?> GetByIdIncludingDeletedAsync(Guid id);
    Task<IEnumerable<Category>> GetAllActiveAsync();
    Task<IEnumerable<Category>> GetAllForAdminAsync();
    Task ToggleCategoryStatusAsync(Category category);
    Category? GetByName(string name);
}