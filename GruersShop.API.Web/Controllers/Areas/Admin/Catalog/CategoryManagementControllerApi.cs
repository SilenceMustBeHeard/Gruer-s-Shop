using GruersShop.Services.Core.Service.Admin.Interfaces.Catalog;
using GruersShop.Web.ViewModels.Admin.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GruersShop.API.Web.Controllers.Areas.Admin.Catalog;

[Route("api/admin/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class CategoryManagementControllerApi : ControllerBase
{
    private readonly ICategoryManagementService _categoryService;

    public CategoryManagementControllerApi(ICategoryManagementService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCategories()
    {
        var categories = await _categoryService.GetAllCategoriesForAdminAsync();
        return Ok(categories);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCategoryById(Guid id)
    {
        var model = await _categoryService.GetCategoryForEditByIdAsync(id);

        if (model == null)
            return NotFound(new { error = "Category not found" });

        return Ok(model);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody] CategoryViewModelCreate model)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });

        await _categoryService.AddCategoryAsync(model);
        return Ok(new { success = true, message = "Category created successfully!" });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> EditCategory(Guid id, [FromBody] CategoryViewModelEdit model)
    {
        if (id != model.Id)
            return BadRequest(new { error = "ID mismatch" });

        if (!ModelState.IsValid)
            return BadRequest(new { errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });

        await _categoryService.EditCategoryAsync(model.Id, model);
        return Ok(new { success = true, message = "Category edited successfully!" });
    }

    [HttpPost("{id}/toggle")]
    public async Task<IActionResult> ToggleActive(Guid id)
    {
        await _categoryService.ToggleCategoryAsync(id);
        return Ok(new { success = true, message = "Category status changed!" });
    }
}