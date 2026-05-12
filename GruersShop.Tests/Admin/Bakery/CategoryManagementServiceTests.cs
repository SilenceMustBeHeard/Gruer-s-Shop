using GruersShop.Data.Models.Base;
using GruersShop.Data.Models.Catalog;
using GruersShop.Data.Models.Interactions;
using GruersShop.Data.Models.Products;
using GruersShop.Data.Repositories.Interfaces.Bakery;
using GruersShop.Services.Core.Service.Admin.Implementations.Catalog;
using GruersShop.Web.ViewModels.Admin.Category;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Assert = NUnit.Framework.Assert;

namespace GruersShop.Unit.Tests.Services.Admin.Bakery;

[TestFixture]
public class CategoryManagementServiceTests
{
    private Mock<ICategoryRepository> _categoryRepoMock; private CategoryManagementService _categoryManagementService;

    private string _testUserId;
    private Guid _testCategoryId;
    private Guid _otherCategoryId;
    private Category _testCategory;
    private Category _otherCategory;
    private Category _deletedCategory;
    private List<Category> _testCategories;

    [SetUp]
    public void SetUp()
    {
        _categoryRepoMock = new Mock<ICategoryRepository>(MockBehavior.Strict);
        _categoryManagementService = new CategoryManagementService(_categoryRepoMock.Object);

        SeedTestData();
        SetupRepositoryMocks();
    }

    private void SeedTestData()
    {
        _testUserId = "user-123";
        _testCategoryId = Guid.NewGuid();
        _otherCategoryId = Guid.NewGuid();

        _testCategory = new Category
        {
            Id = _testCategoryId,
            Name = "Bread",
            Description = "Fresh baked breads",
            DisplayOrder = 2,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow.AddDays(-5),
            Products = new List<Product>
            {
                new Product { Id = Guid.NewGuid(), Name = "Sourdough", IsAvailable = true, IsDeleted = false },
                new Product { Id = Guid.NewGuid(), Name = "Baguette", IsAvailable = true, IsDeleted = false }
            }
        };

        _otherCategory = new Category
        {
            Id = _otherCategoryId,
            Name = "Croissant",
            Description = "Fresh baked croissants",
            DisplayOrder = 1,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow.AddDays(-3),
            Products = new List<Product>()
        };

        _deletedCategory = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Deleted Category",
            IsDeleted = true,
            Products = new List<Product>()
        };

        _testCategories = new List<Category> { _testCategory, _otherCategory, _deletedCategory };
    }

    private void SetupRepositoryMocks()
    {
        var activeCategories = _testCategories.Where(c => !c.IsDeleted).ToList();
        _categoryRepoMock.Setup(x => x.GetAllActiveAsync())
            .ReturnsAsync(activeCategories);

        _categoryRepoMock.Setup(x => x.GetAllForAdminAsync())
            .ReturnsAsync(_testCategories);

        _categoryRepoMock.Setup(x => x.GetByIdAsync(_testCategoryId))
            .ReturnsAsync(_testCategory);
        _categoryRepoMock.Setup(x => x.GetByIdAsync(_otherCategoryId))
            .ReturnsAsync(_otherCategory);
        _categoryRepoMock.Setup(x => x.GetByIdAsync(It.Is<Guid>(id => id != _testCategoryId && id != _otherCategoryId)))
            .ReturnsAsync((Category)null);

        _categoryRepoMock.Setup(x => x.GetByIdIncludingDeletedAsync(_testCategoryId))
            .ReturnsAsync(_testCategory);
        _categoryRepoMock.Setup(x => x.GetByIdIncludingDeletedAsync(_deletedCategory.Id))
            .ReturnsAsync(_deletedCategory);
        _categoryRepoMock.Setup(x => x.GetByIdIncludingDeletedAsync(It.Is<Guid>(id => id != _testCategoryId && id != _deletedCategory.Id)))
            .ReturnsAsync((Category)null);

        var defaultCatalog = new Catalog { Id = Guid.NewGuid(), Name = "Default" };
        _categoryRepoMock.Setup(x => x.GetDefaultCatalogAsync())
            .ReturnsAsync(defaultCatalog);

        _categoryRepoMock.Setup(x => x.AddAsync(It.IsAny<Category>()))
            .Returns(Task.CompletedTask);
        _categoryRepoMock.Setup(x => x.UpdateAsync(It.IsAny<Category>()))
            .ReturnsAsync(true);
        _categoryRepoMock.Setup(x => x.SaveChangesAsync())
            .Returns(Task.CompletedTask);
        _categoryRepoMock.Setup(x => x.ToggleCategoryStatusAsync(It.IsAny<Category>()))
            .Returns(Task.CompletedTask);
    }

    #region GetAllActiveCategoriesAsync Tests

    [Test]
    public async Task GetAllActiveCategoriesAsync_ReturnsOnlyActiveCategories()
    {
        var result = await _categoryManagementService.GetAllActiveCategoriesAsync();
        var resultList = result.ToList();

        Assert.That(resultList.Count, Is.EqualTo(2));
        Assert.That(resultList.Any(c => c.Name == "Deleted Category"), Is.False);
    }

    [Test]
    public async Task GetAllActiveCategoriesAsync_ReturnsCategoriesWithProductCount()
    {
        var result = await _categoryManagementService.GetAllActiveCategoriesAsync();
        var breadCategory = result.FirstOrDefault(c => c.Name == "Bread");

        Assert.That(breadCategory, Is.Not.Null);
        Assert.That(breadCategory.ProductCount, Is.EqualTo(2));
    }

    #endregion

    #region GetAllCategoriesForAdminAsync Tests

    [Test]
    public async Task GetAllCategoriesForAdminAsync_IncludesDeletedCategories()
    {
        var result = await _categoryManagementService.GetAllCategoriesForAdminAsync();
        var resultList = result.ToList();

        Assert.That(resultList.Count, Is.EqualTo(3));
        Assert.That(resultList.Any(c => c.Name == "Deleted Category"), Is.True);
    }

    #endregion

    #region GetCategoryByIdAsync Tests

    [Test]
    public async Task GetCategoryByIdAsync_ExistingCategory_ReturnsCategory()
    {
        var result = await _categoryManagementService.GetCategoryByIdAsync(_testCategoryId);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(_testCategoryId));
        Assert.That(result.Name, Is.EqualTo("Bread"));
    }

    [Test]
    public async Task GetCategoryByIdAsync_NonExistentCategory_ReturnsNull()
    {
        var result = await _categoryManagementService.GetCategoryByIdAsync(Guid.NewGuid());

        Assert.That(result, Is.Null);
    }

    #endregion

    #region GetCategoryForEditByIdAsync Tests

    [Test]
    public async Task GetCategoryForEditByIdAsync_ExistingCategory_ReturnsEditModel()
    {
        var result = await _categoryManagementService.GetCategoryForEditByIdAsync(_testCategoryId);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(_testCategoryId));
        Assert.That(result.Name, Is.EqualTo("Bread"));
        Assert.That(result.IsDeleted, Is.False);
    }

    [Test]
    public async Task GetCategoryForEditByIdAsync_DeletedCategory_ReturnsEditModel()
    {
        var result = await _categoryManagementService.GetCategoryForEditByIdAsync(_deletedCategory.Id);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(_deletedCategory.Id));
        Assert.That(result.IsDeleted, Is.True);
    }

    [Test]
    public async Task GetCategoryForEditByIdAsync_NonExistentCategory_ReturnsNull()
    {
        var result = await _categoryManagementService.GetCategoryForEditByIdAsync(Guid.NewGuid());

        Assert.That(result, Is.Null);
    }

    #endregion

    #region AddCategoryAsync Tests

    [Test]
    public async Task AddCategoryAsync_CreatesNewCategory()
    {
        var newCategory = new CategoryViewModelCreate
        {
            Name = "New Category",
            Description = "New Description"
        };

        await _categoryManagementService.AddCategoryAsync(newCategory);

        _categoryRepoMock.Verify(x => x.AddAsync(It.Is<Category>(c =>
            c.Name == "New Category" &&
            c.Description == "New Description" &&
            c.IsDeleted == false)), Times.Once);
        _categoryRepoMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    #endregion

    #region EditCategoryAsync Tests

    [Test]
    public async Task EditCategoryAsync_UpdatesExistingCategory()
    {
        var editModel = new CategoryViewModelEdit
        {
            Id = _testCategoryId,
            Name = "Updated Name",
            Description = "Updated Description",
            IsDeleted = true
        };

        await _categoryManagementService.EditCategoryAsync(_testCategoryId, editModel);

        _categoryRepoMock.Verify(x => x.UpdateAsync(It.Is<Category>(c =>
            c.Id == _testCategoryId &&
            c.Name == "Updated Name" &&
            c.Description == "Updated Description" &&
            c.IsDeleted == true)), Times.Once);
        _categoryRepoMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task EditCategoryAsync_NonExistentCategory_DoesNothing()
    {
        var editModel = new CategoryViewModelEdit
        {
            Id = Guid.NewGuid(),
            Name = "Updated Name",
            Description = "Updated Description",
            IsDeleted = false
        };

        await _categoryManagementService.EditCategoryAsync(editModel.Id, editModel);

        _categoryRepoMock.Verify(x => x.UpdateAsync(It.IsAny<Category>()), Times.Never);
        _categoryRepoMock.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    #endregion

    #region ToggleCategoryAsync Tests

    [Test]
    public async Task ToggleCategoryAsync_TogglesCategoryStatus()
    {
        await _categoryManagementService.ToggleCategoryAsync(_testCategoryId);

        _categoryRepoMock.Verify(x => x.ToggleCategoryStatusAsync(It.Is<Category>(c =>
            c.Id == _testCategoryId)), Times.Once);
        _categoryRepoMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task ToggleCategoryAsync_NonExistentCategory_DoesNothing()
    {
        await _categoryManagementService.ToggleCategoryAsync(Guid.NewGuid());

        _categoryRepoMock.Verify(x => x.ToggleCategoryStatusAsync(It.IsAny<Category>()), Times.Never);
        _categoryRepoMock.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    #endregion
}