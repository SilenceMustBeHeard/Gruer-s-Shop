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
public class CatalogClientServiceTests
{
    private Mock<IUnitOfWork> _unitOfWorkMock;
    private CatalogClientService _catalogClientService;

    private string _testUserId;
    private string _otherUserId;
    private Guid _testProductId;
    private Guid _otherProductId;
    private Guid _testCategoryId;
    private Category _testCategory;
    private Product _testProduct;
    private Product _testProduct2;
    private List<Product> _testProducts;

    [SetUp]
    public void SetUp()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>(MockBehavior.Strict);
        _catalogClientService = new CatalogClientService(_unitOfWorkMock.Object);

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

        _testCategory = new Category
        {
            Id = _testCategoryId,
            Name = "Bread",
            Description = "Fresh baked breads",
            DisplayOrder = 1,
            IsDeleted = false
        };

        _testProduct = new Product
        {
            Id = _testProductId,
            Name = "Sourdough Bread",
            Description = "A delicious sourdough bread. This is a long description that might need truncation in the view model.",
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
                UserId = "other-user",
                User = new AppUser { Id = "other-user", FirstName = "Other", LastName = "User" },
                Rating = 4,
                Comment = "Very good",
                CreatedAt = DateTime.UtcNow.AddDays(-3)
            }
        },
            FavoritedBy = new List<Favorite>()
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
                Comment = "Excellent bread!",
                CreatedAt = DateTime.UtcNow.AddDays(-2)
            },
            new Review
            {
                Id = Guid.NewGuid(),
                UserId = "other-user",
                User = new AppUser { Id = "other-user", FirstName = "Other", LastName = "User" },
                Rating = 4,
                Comment = "Very good",
                CreatedAt = DateTime.UtcNow.AddDays(-3)
            }
            },
            FavoritedBy = new List<Favorite>()
        };

        _testProducts = new List<Product> { _testProduct, _testProduct2 };
    }

    private void SetupRepositoryMocks()
    {
        var productRepoMock = new Mock<IFullRepositoryAsync<Product, Guid>>(MockBehavior.Strict);

        var mockDbSet = _testProducts.BuildMockDbSet();

        productRepoMock.Setup(x => x.Query()).Returns(mockDbSet.Object);

        _unitOfWorkMock.Setup(x => x.Repository<Product, Guid>())
            .Returns(productRepoMock.Object);
    }

    #region GetPublicCatalogAsync Tests

    [Test]
    public async Task GetPublicCatalogAsync_GuestUser_ReturnsTop3Products()
    {
        var page = 1;
        var pageSize = 12;
        var isGuest = true;
        Guid? categoryId = null;

        var result = await _catalogClientService.GetPublicCatalogAsync(
            null, page, pageSize, isGuest, categoryId);

        var resultList = result.ToList();

        Assert.That(resultList.Count, Is.EqualTo(2));
    }

    [Test]
    public async Task GetPublicCatalogAsync_LoggedInUser_ReturnsPaginatedProducts()
    {
        var page = 1;
        var pageSize = 12;
        var isGuest = false;
        Guid? categoryId = null;

        var result = await _catalogClientService.GetPublicCatalogAsync(
            _testUserId, page, pageSize, isGuest, categoryId);

        var resultList = result.ToList();

        Assert.That(resultList.Count, Is.EqualTo(2));
    }

    [Test]
    public async Task GetPublicCatalogAsync_WithCategoryFilter_ReturnsFilteredProducts()
    {
        var page = 1;
        var pageSize = 12;

        var isGuest = false;

        Guid? categoryId = _testCategoryId;

        var result = await _catalogClientService.GetPublicCatalogAsync(
            _testUserId, page, pageSize, isGuest, categoryId);
        var resultList = result.ToList();

        Assert.That(resultList.Count, Is.EqualTo(2));
    }

    #endregion GetPublicCatalogAsync Tests

    #region GetTotalActiveProductsAsync Tests

    [Test]
    public async Task GetTotalActiveProductsAsync_ReturnsCorrectCount()
    {
        Guid? categoryId = null;

        var result = await _catalogClientService.GetTotalActiveProductsAsync(categoryId);

        Assert.That(result, Is.EqualTo(2));
    }

    [Test]
    public async Task GetTotalActiveProductsAsync_WithCategoryFilter_ReturnsFilteredCount()
    {
        Guid? categoryId = _testCategoryId;

        var result = await _catalogClientService.GetTotalActiveProductsAsync(categoryId);

        Assert.That(result, Is.EqualTo(2));
    }

    [Test]
    public async Task GetTotalActiveProductsAsync_NoActiveProducts_ReturnsZero()
    {
        var emptyProductList = new List<Product>().BuildMockDbSet();
        var productRepoMock = new Mock<IFullRepositoryAsync<Product, Guid>>(MockBehavior.Strict);

        productRepoMock.Setup(x => x.Query()).Returns(emptyProductList.Object);
        _unitOfWorkMock.Setup(x => x.Repository<Product, Guid>())
            .Returns(productRepoMock.Object);

        Guid? categoryId = null;

        var result = await _catalogClientService.GetTotalActiveProductsAsync(categoryId);
        Assert.That(result, Is.EqualTo(0));
    }

    #endregion GetTotalActiveProductsAsync Tests

    #region GetProductDetailsAsync Tests

    [Test]
    public async Task GetProductDetailsAsync_ExistingProduct_ReturnsProductDetails()
    {
        var result = await _catalogClientService.GetProductDetailsAsync(_testProductId, _testUserId);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(_testProductId));
        Assert.That(result.Name, Is.EqualTo(_testProduct.Name));
        Assert.That(result.Description, Is.EqualTo(_testProduct.Description));
        Assert.That(result.Price, Is.EqualTo(_testProduct.Price));
        Assert.That(result.StockQuantity, Is.EqualTo(_testProduct.StockQuantity));
        Assert.That(result.IsInStock, Is.EqualTo(_testProduct.IsAvailable));
        Assert.That(result.CategoryId, Is.EqualTo(_testCategoryId));
        Assert.That(result.CategoryName, Is.EqualTo(_testCategory.Name));
        Assert.That(result.AverageRating, Is.EqualTo(_testProduct.AverageRating));

        Assert.That(result.Reviews.Count, Is.EqualTo(_testProduct.Reviews.Count));
    }

    [Test]
    public async Task GetProductDetailsAsync_NonExistingProduct_ReturnsNull()
    {
        var nonExistingProductId = Guid.NewGuid();
        var result = await _catalogClientService.GetProductDetailsAsync(nonExistingProductId, _testUserId);
        Assert.That(result, Is.Null);
    }

    #endregion GetProductDetailsAsync Tests

    #region GetProductDetailsViewModelAsync Tests

    [Test]
    public async Task GetProductDetailsViewModelAsync_ExistingProduct_ReturnsViewModel()
    {
        var result = await _catalogClientService.GetProductDetailsViewModelAsync(_testProductId, _testUserId);

        Assert.That(_testProduct.Reviews.Count, Is.EqualTo(2));
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Product.Id, Is.EqualTo(_testProductId));
        Assert.That(result.UserHasReviewed, Is.True);
        Assert.That(result.UserReview, Is.Not.Null);
        Assert.That(result.UserReview.Rating, Is.EqualTo(5));
        Assert.That(result.UserReview.Comment, Is.EqualTo("Excellent bread!"));
        Assert.That(result.RelatedProducts.Count(), Is.EqualTo(1));
    }

    [Test]
    public async Task GetProductDetailsViewModelAsync_NonExistingProduct_ReturnsNull()
    {
        var nonExistingProductId = Guid.NewGuid();
        var result = await _catalogClientService.GetProductDetailsViewModelAsync(nonExistingProductId, _testUserId);
        Assert.That(result, Is.Null);
    }

    #endregion GetProductDetailsViewModelAsync Tests

    #region GetCategoriesForNavAsync Tests

    [Test]
    public async Task GetCategoriesForNavAsync_ReturnsAllCategories()
    {
        var categoryRepoMock = new Mock<IFullRepositoryAsync<Category, Guid>>(MockBehavior.Strict);
        var categories = new List<Category>
        {
            _testCategory,
            new Category
            {
                Id = Guid.NewGuid(),
                Name = "Pastries",
                Description = "Delicious pastries",
                DisplayOrder = 2,
                IsDeleted = false
            }
        }.BuildMockDbSet();
        categoryRepoMock.Setup(x => x.Query()).Returns(categories.Object);
        _unitOfWorkMock.Setup(x => x.Repository<Category, Guid>())
            .Returns(categoryRepoMock.Object);
        var result = await _catalogClientService.GetCategoriesForNavAsync();
        var resultList = result.ToList();
        Assert.That(resultList.Count, Is.EqualTo(2));
        Assert.That(resultList[0].Name, Is.EqualTo("Bread"));
        Assert.That(resultList[1].Name, Is.EqualTo("Pastries"));
    }

    [Test]
    public async Task GetCategoriesForNavAsync_NoCategories_ReturnsEmptyList()
    {
        var emptyCategoryList = new List<Category>().BuildMockDbSet();
        var categoryRepoMock = new Mock<IFullRepositoryAsync<Category, Guid>>(MockBehavior.Strict);
        categoryRepoMock.Setup(x => x.Query()).Returns(emptyCategoryList.Object);
        _unitOfWorkMock.Setup(x => x.Repository<Category, Guid>())
            .Returns(categoryRepoMock.Object);
        var result = await _catalogClientService.GetCategoriesForNavAsync();
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count(), Is.EqualTo(0));
    }

    #endregion GetCategoriesForNavAsync Tests
}