using GruersShop.Data.Models.Base;
using GruersShop.Data.Models.Catalog;
using GruersShop.Data.Models.Interactions;
using GruersShop.Data.Models.Products;
using GruersShop.Data.Repositories.Interfaces.CRUD;
using GruersShop.Services.Core.Service.Implementations.Bakery;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace GruersShop.Unit.Tests.Services.User.Bakery;

[TestFixture]
public class CategoryClientServiceTests
{
    private Mock<IUnitOfWork> _unitOfWorkMock;
    private CategoryClientService _categoryClientService;

    private string _testUserId;
    private string _otherUserId;
    private Guid _testProductId;
    private Guid _otherProductId;
    private Guid _testCategoryId;
    private Guid _otherCategoryId;
    private Category _testCategory;
    private Category _otherCategory;
    private Product _testProduct;
    private Product _testProduct2;
    private List<Category> _testCategories;

    [SetUp]
    public void SetUp()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>(MockBehavior.Strict);
        _categoryClientService = new CategoryClientService(_unitOfWorkMock.Object);

        SeedTestData();
        SetupRepositoryMocks();
    }

    private void SeedTestData()
    {
        _testUserId = "user-123";
        _otherUserId = "user-456";
        _testProductId = Guid.NewGuid();
        _otherProductId = Guid.NewGuid();
        _testCategoryId = Guid.NewGuid();
        _otherCategoryId = Guid.NewGuid();

        _testCategory = new Category
        {
            Id = _testCategoryId,
            Name = "Bread",
            Description = "Fresh baked breads",
            DisplayOrder = 2,
            IsDeleted = false,
            Products = new List<Product>()
        };

        _otherCategory = new Category
        {
            Id = _otherCategoryId,
            Name = "Croissant",
            Description = "Fresh baked croissants",
            DisplayOrder = 1,
            IsDeleted = false,
            Products = new List<Product>()
        };

        _testProduct = new Product
        {
            Id = _testProductId,
            Name = "Sourdough Bread",
            Description = "A delicious sourdough bread.",
            Price = 5.99m,
            StockQuantity = 100,
            IsAvailable = true,
            IsDeleted = false,
            CategoryId = _testCategoryId,
            Category = _testCategory,
            AverageRating = 4.5,
            CreatedAt = DateTime.UtcNow.AddDays(-5),
            Reviews = new List<Review>
            {
                new Review
                {
                    Id = Guid.NewGuid(),
                    UserId = _testUserId,
                    User = new AppUser { Id = _testUserId, FirstName = "Test", LastName = "User" },
                    Rating = 5,
                    Comment = "Excellent bread!",
                    CreatedAt = DateTime.UtcNow.AddDays(-2)
                },
                new Review
                {
                    Id = Guid.NewGuid(),
                    UserId = _otherUserId,
                    User = new AppUser { Id = _otherUserId, FirstName = "Other", LastName = "User" },
                    Rating = 4,
                    Comment = "Very good",
                    CreatedAt = DateTime.UtcNow.AddDays(-3)
                }
            },
            FavoritedBy = new List<Favorite>
            {
                new Favorite { UserId = _testUserId, IsDeleted = false }
            }
        };

        _testProduct2 = new Product
        {
            Id = _otherProductId,
            Name = "Baguette",
            Description = "A classic French baguette.",
            Price = 3.99m,
            StockQuantity = 50,
            IsAvailable = true,
            IsDeleted = false,
            CategoryId = _testCategoryId,
            Category = _testCategory,
            AverageRating = 4.0,
            CreatedAt = DateTime.UtcNow.AddDays(-2),
            Reviews = new List<Review>
            {
                new Review
                {
                    Id = Guid.NewGuid(),
                    UserId = _testUserId,
                    User = new AppUser { Id = _testUserId, FirstName = "Test", LastName = "User" },
                    Rating = 4,
                    Comment = "Good bread!",
                    CreatedAt = DateTime.UtcNow.AddDays(-2)
                }
            },
            FavoritedBy = new List<Favorite>()
        };

        _testCategory.Products = new List<Product> { _testProduct, _testProduct2 };
        _otherCategory.Products = new List<Product>();

        _testCategories = new List<Category> { _testCategory, _otherCategory };
    }

    private void SetupRepositoryMocks()
    {
        var categoryRepoMock = new Mock<IFullRepositoryAsync<Category, Guid>>(MockBehavior.Strict);

        var mockDbSet = _testCategories.BuildMockDbSet();
        categoryRepoMock.Setup(x => x.Query()).Returns(mockDbSet.Object);

        _unitOfWorkMock.Setup(x => x.Repository<Category, Guid>())
            .Returns(categoryRepoMock.Object);
    }

    #region GetAllActiveCategoriesAsync Tests

    [Test]
    public async Task GetAllActiveCategoriesAsync_ReturnsOnlyActiveCategories()
    {
        var deletedCategory = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Deleted Category",
            IsDeleted = true,
            Products = new List<Product>()
        };

        var categoriesWithDeleted = new List<Category> { _testCategory, _otherCategory, deletedCategory };
        var mockDbSet = categoriesWithDeleted.BuildMockDbSet();

        var categoryRepoMock = new Mock<IFullRepositoryAsync<Category, Guid>>();
        categoryRepoMock.Setup(x => x.Query()).Returns(mockDbSet.Object);
        _unitOfWorkMock.Setup(x => x.Repository<Category, Guid>()).Returns(categoryRepoMock.Object);

        var result = await _categoryClientService.GetAllActiveCategoriesAsync();
        var resultList = result.ToList();

        Assert.That(resultList.Count, Is.EqualTo(2));
        Assert.That(resultList.Any(c => c.Name == "Deleted Category"), Is.False);
    }

    [Test]
    public async Task GetAllActiveCategoriesAsync_ReturnsCategoriesWithCorrectProductCount()
    {
        var result = await _categoryClientService.GetAllActiveCategoriesAsync();
        var breadCategory = result.FirstOrDefault(c => c.Id == _testCategoryId);

        Assert.That(breadCategory, Is.Not.Null);
        Assert.That(breadCategory.ProductCount, Is.EqualTo(2));
    }

    [Test]
    public async Task GetAllActiveCategoriesAsync_ReturnsCategoriesOrderedByDisplayOrder()
    {
        var result = await _categoryClientService.GetAllActiveCategoriesAsync();
        var resultList = result.ToList();

        Assert.That(resultList[0].Id, Is.EqualTo(_otherCategoryId)); Assert.That(resultList[1].Id, Is.EqualTo(_testCategoryId));
    }

    #endregion GetAllActiveCategoriesAsync Tests

    #region GetCategoryByIdAsync Tests

    [Test]
    public async Task GetCategoryByIdAsync_ExistingCategory_ReturnsCategoryWithProductCount()
    {
        var result = await _categoryClientService.GetCategoryByIdAsync(_testCategoryId);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(_testCategoryId));
        Assert.That(result.Name, Is.EqualTo("Bread"));
        Assert.That(result.ProductCount, Is.EqualTo(2));
    }

    [Test]
    public async Task GetCategoryByIdAsync_DeletedCategory_ReturnsNull()
    {
        var deletedCategory = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Deleted",
            IsDeleted = true,
            Products = new List<Product>()
        };

        var categoriesWithDeleted = new List<Category> { _testCategory, _otherCategory, deletedCategory };
        var mockDbSet = categoriesWithDeleted.BuildMockDbSet();

        var categoryRepoMock = new Mock<IFullRepositoryAsync<Category, Guid>>();
        categoryRepoMock.Setup(x => x.Query()).Returns(mockDbSet.Object);
        _unitOfWorkMock.Setup(x => x.Repository<Category, Guid>()).Returns(categoryRepoMock.Object);

        var result = await _categoryClientService.GetCategoryByIdAsync(deletedCategory.Id);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetCategoryByIdAsync_NonExistentCategory_ReturnsNull()
    {
        var result = await _categoryClientService.GetCategoryByIdAsync(Guid.NewGuid());

        Assert.That(result, Is.Null);
    }

    #endregion GetCategoryByIdAsync Tests

    #region GetCategoriesWithProductCountAsync Tests

    [Test]
    public async Task GetCategoriesWithProductCountAsync_ReturnsOnlyCategoriesWithProducts()
    {
        var result = await _categoryClientService.GetCategoriesWithProductCountAsync();
        var resultList = result.ToList();

        Assert.That(resultList.Count, Is.EqualTo(1)); Assert.That(resultList[0].Id, Is.EqualTo(_testCategoryId));
        Assert.That(resultList[0].Name, Is.EqualTo("Bread"));
    }

    [Test]
    public async Task GetCategoriesWithProductCountAsync_ReturnsCategoriesOrderedByDisplayOrder()
    {
        var result = await _categoryClientService.GetCategoriesWithProductCountAsync();
        var resultList = result.ToList();

        Assert.That(resultList[0].Id, Is.EqualTo(_testCategoryId));
    }

    [Test]
    public async Task GetCategoriesWithProductCountAsync_ReturnsCorrectProductCount()
    {
        var result = await _categoryClientService.GetCategoriesWithProductCountAsync();
        var breadCategory = result.FirstOrDefault();

        Assert.That(breadCategory, Is.Not.Null);
        Assert.That(breadCategory.ProductCount, Is.EqualTo(2));
    }

    #endregion GetCategoriesWithProductCountAsync Tests

    #region GetCategoryWithProductsAsync Tests

    [Test]
    public async Task GetCategoryWithProductsAsync_ExistingCategory_ReturnsCategoryWithProducts()
    {
        var result = await _categoryClientService.GetCategoryWithProductsAsync(_testCategoryId, null);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(_testCategoryId));
        Assert.That(result.Name, Is.EqualTo("Bread"));
        Assert.That(result.Products.Count(), Is.EqualTo(2));
        Assert.That(result.TotalProducts, Is.EqualTo(2));
    }

    [Test]
    public async Task GetCategoryWithProductsAsync_WhenUserIsLoggedIn_MarksFavoritedProducts()
    {
        var result = await _categoryClientService.GetCategoryWithProductsAsync(_testCategoryId, _testUserId);
        var sourdoughProduct = result.Products.FirstOrDefault(p => p.Name == "Sourdough Bread");

        Assert.That(sourdoughProduct, Is.Not.Null);
        Assert.That(sourdoughProduct.IsFavorited, Is.True);
    }

    [Test]
    public async Task GetCategoryWithProductsAsync_WhenUserIsGuest_NoProductsMarkedFavorited()
    {
        var result = await _categoryClientService.GetCategoryWithProductsAsync(_testCategoryId, null);
        var sourdoughProduct = result.Products.FirstOrDefault(p => p.Name == "Sourdough Bread");

        Assert.That(sourdoughProduct, Is.Not.Null);
        Assert.That(sourdoughProduct.IsFavorited, Is.False);
    }

    [Test]
    public async Task GetCategoryWithProductsAsync_DeletedCategory_ReturnsNull()
    {
        var deletedCategory = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Deleted",
            IsDeleted = true,
            Products = new List<Product>()
        };

        var categoriesWithDeleted = new List<Category> { _testCategory, _testCategory, deletedCategory };
        var mockDbSet = categoriesWithDeleted.BuildMockDbSet();

        var categoryRepoMock = new Mock<IFullRepositoryAsync<Category, Guid>>();
        categoryRepoMock.Setup(x => x.Query()).Returns(mockDbSet.Object);
        _unitOfWorkMock.Setup(x => x.Repository<Category, Guid>()).Returns(categoryRepoMock.Object);

        var result = await _categoryClientService.GetCategoryWithProductsAsync(deletedCategory.Id, null);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetCategoryWithProductsAsync_NonExistentCategory_ReturnsNull()
    {
        var result = await _categoryClientService.GetCategoryWithProductsAsync(Guid.NewGuid(), null);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetCategoryWithProductsAsync_CalculatesAverageRatingCorrectly()
    {
        var result = await _categoryClientService.GetCategoryWithProductsAsync(_testCategoryId, null);
        var sourdoughProduct = result.Products.FirstOrDefault(p => p.Name == "Sourdough Bread");

        Assert.That(sourdoughProduct, Is.Not.Null);
        Assert.That(sourdoughProduct.AverageRating, Is.EqualTo(4.5));
    }

    #endregion GetCategoryWithProductsAsync Tests
}