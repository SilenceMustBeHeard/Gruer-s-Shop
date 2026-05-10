using GruersShop.Data.Models.Catalog;
using GruersShop.Data.Models.Interactions;
using GruersShop.Data.Models.Products;
using GruersShop.Data.Repositories.Interfaces.Bakery;
using GruersShop.Data.Repositories.Interfaces.Interactions;
using GruersShop.Services.Core.Service.Implementations.Interactions;
using GruersShop.Services.Core.Service.Interfaces.Interactions;
using Moq;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using MockQueryable.Moq;

namespace GruersShop.Unit.Tests.Services.User.Interactions;

[TestFixture]
public class FavoriteServiceTests
{
    private Mock<IFavoriteRepository> _favoriteRepoMock;
    private Mock<IProductRepository> _productRepoMock;
    private FavoriteService _favoriteService;


    private string _testUserId;
    private string _otherUserId;
    private Guid _testProductId;
    private Guid _otherProductId;
    private Favorite _existingFavorite;
    private Favorite _otherUserFavorite;
    private Product _testProduct;
    private List<Favorite> _testFavorites;
    private List<Product> _testProducts;



    [SetUp]
    public void SetUp()
    {
        _favoriteRepoMock = new Mock<IFavoriteRepository>(MockBehavior.Strict);
        _productRepoMock = new Mock<IProductRepository>(MockBehavior.Strict);
        _favoriteService = new FavoriteService(_favoriteRepoMock.Object);
        SeedTestData();
    }


    private void SeedTestData()
    {
        _testUserId = "test-user-123";
        _otherUserId = "other-user-456";
        _testProductId = Guid.NewGuid();
        _otherProductId = Guid.NewGuid();

   
        var testCategory = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Test Category",
            Description = "Category for testing",
            DisplayOrder = 1,
            IsDeleted = false
        };

        _testProduct = new Product
        {
            Id = _testProductId,
            Name = "Test Product",
            Description = "A product for testing",
            Price = 9.99m,
            StockQuantity = 100,
            IsDeleted = false,
            IsAvailable = true,
            CategoryId = testCategory.Id,
            Category = testCategory,           
            AverageRating = 4.5,
            Reviews = new List<Review>()      
        };

        _existingFavorite = new Favorite()
        {
            Id = Guid.NewGuid(),
            UserId = _testUserId,
            ProductId = _testProductId,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            IsDeleted = false,
            Product = _testProduct            
        };

        _otherUserFavorite = new Favorite
        {
            Id = Guid.NewGuid(),
            UserId = _otherUserId,
            ProductId = _testProductId,
            CreatedAt = DateTime.UtcNow.AddDays(-2),
            IsDeleted = false,
            Product = _testProduct
        };

        _testFavorites = new List<Favorite> { _existingFavorite, _otherUserFavorite };
        _testProducts = new List<Product> { _testProduct };
    }


    #region ToggleFavoriteAsync Tests


    [Test]
    public async Task ToggleFavoriteAsync_ShouldAddFavorite_WhenNotExisting()
    {

        _favoriteRepoMock.Setup(r => r.GetByCompositeKeyAsync(_testUserId, _testProductId))
            .ReturnsAsync((Favorite?)null);

        _favoriteRepoMock.Setup(r => r.AddAsync(It.IsAny<Favorite>()))
            .Returns(Task.CompletedTask);

        _favoriteRepoMock.Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);


        var result = await _favoriteService.ToggleFavoriteAsync(_testUserId, _testProductId);


        Assert.IsTrue(result);

        _favoriteRepoMock.Verify(r => r.AddAsync(It.Is<Favorite>(f =>
            f.UserId == _testUserId &&
            f.ProductId == _testProductId)), Times.Once);
        _favoriteRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task ToggleFavoriteAsync_ShouldRemoveFavorite_WhenExisting()
    {
        _favoriteRepoMock.Setup(r => r.GetByCompositeKeyAsync(_testUserId, _testProductId))
            .ReturnsAsync(_existingFavorite);

        _favoriteRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Favorite>()))
            .ReturnsAsync(true);

        _favoriteRepoMock.Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);


        var result = await _favoriteService.ToggleFavoriteAsync(_testUserId, _testProductId);

        Assert.IsFalse(result);

        _favoriteRepoMock.Verify(r => r.UpdateAsync(It.Is<Favorite>(f =>
            f.Id == _existingFavorite.Id &&
            f.UserId == _testUserId &&
            f.ProductId == _testProductId &&
            f.IsDeleted)), Times.Once);
        _favoriteRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);






    }

    #endregion


    #region GetUserFavoritesAsync Tests


  





[Test]
public async Task GetUserFavoritesAsync_ShouldReturnFavorites_ForUser()
{

    var mockDbSet = _testFavorites.BuildMockDbSet();

    _favoriteRepoMock.Setup(r => r.Query())
        .Returns(mockDbSet.Object);


    var result = await _favoriteService.GetUserFavoritesAsync(_testUserId);
    var favoritesList = result.ToList();

 
    Assert.That(result, Is.Not.Null);
    Assert.That(favoritesList.Count, Is.EqualTo(1));
    Assert.That(favoritesList[0].Id, Is.EqualTo(_testProductId));
    Assert.That(favoritesList[0].Name, Is.EqualTo("Test Product"));
    Assert.That(favoritesList[0].CategoryName, Is.EqualTo("Test Category"));
}









    [Test]


    public async Task GetUserFavoritesAsync_ShouldReturnEmpty_WhenNoFavorites()
    {
        var mockDbSet = new List<Favorite>().BuildMockDbSet();
        _favoriteRepoMock.Setup(r => r.Query())
            .Returns(mockDbSet.Object);

        var result = await _favoriteService.GetUserFavoritesAsync(_testUserId);
        var favoritesList = result.ToList();

        Assert.That(favoritesList, Is.Empty);
    }

    #endregion

    #region IsFavoriteAsync Tests



    [Test]

    public async Task IsFavoriteAsync_ShouldReturnTrue_WhenFavoriteExists()
    {
        _favoriteRepoMock.Setup(r => r.ExistsAsync(_testUserId, _testProductId))
            .ReturnsAsync(true);
        var result = await _favoriteService.IsFavoriteAsync(_testUserId, _testProductId);
        Assert.IsTrue(result);
    }

    [Test]
    public async Task IsFavoriteAsync_ShouldReturnFalse_WhenFavoriteDoesNotExist()
    {
        _favoriteRepoMock.Setup(r => r.ExistsAsync(_testUserId, _testProductId))
            .ReturnsAsync(false);
        var result = await _favoriteService.IsFavoriteAsync(_testUserId, _testProductId);
        Assert.IsFalse(result);
    } 
    














    #endregion












}







