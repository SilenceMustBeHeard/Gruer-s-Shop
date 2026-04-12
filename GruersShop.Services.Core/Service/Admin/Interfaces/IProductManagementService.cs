using GruersShop.Services.Core.Service.Interfaces.Bakery;
using GruersShop.Web.ViewModels.Admin.Bakery;
using System;
using System.Collections.Generic;
using System.Text;

namespace GruersShop.Services.Core.Service.Admin.Interfaces;

public interface IProductManagementService : ICatalogClientService
{
    Task<IEnumerable<ProductViewModelList>> GetAllActiveProductsAsync();

    Task AddProductAsync(ProductViewModelCreate model);

    Task<ProductViewModelEdit?> GetProductForEditByIdAsync(Guid id);
    Task EditProductAsync(Guid id, ProductViewModelEdit model);

    Task ToggleProductAsync(Guid id);

    Task<IEnumerable<ProductViewModelList>> GetAllProductsForAdminAsync();
}