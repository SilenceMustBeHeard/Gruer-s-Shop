using GruersShop.Data.Models.Interactions;
using GruersShop.Data.Models.Products;
using GruersShop.Data.Repositories.Interfaces.Bakery;
using GruersShop.Data.Repositories.Interfaces.Interactions;
using GruersShop.Services.Core.Service.Implementations.Interactions;
using GruersShop.Web.ViewModels.Interactions;
using Moq;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace GruersShop.Unit.Tests.Services.User.Interactions;

[TestFixture]
public class ReviewServiceTests
{
    private Mock<IReviewRepository> _reviewRepoMock;
    private Mock<IProductRepository> _productRepoMock;
    private ReviewService _reviewService;

    private string _testUserId;
    private string _otherUserId;
    private Guid _testProductId;
    private Guid _otherProductId;
    private Review _existingReview;
    private Review _otherUserReview;
    private Product _testProduct;
    private List<Review> _testReviews;
    private List<Product> _testProducts;

    [SetUp]
    public void SetUp()
    {
        _reviewRepoMock = new Mock<IReviewRepository>(MockBehavior.Strict);
        _productRepoMock = new Mock<IProductRepository>(MockBehavior.Strict);
        _reviewService = new ReviewService(_reviewRepoMock.Object, _productRepoMock.Object);
        SeedTestData();
    }

    private void SeedTestData()
    {
        _testUserId = "test-user-123";
        _otherUserId = "other-user-456";
        _testProductId = Guid.NewGuid();
        _otherProductId = Guid.NewGuid();

        _testProduct = new Product
        {
            Id = _testProductId,
            Name = "Test Product",
            Description = "A product for testing",
            Price = 9.99m,
            StockQuantity = 100,
            IsDeleted = false
        };
        _existingReview = new Review
        {
            Id = Guid.NewGuid(),
            UserId = _testUserId,
            ProductId = _testProductId,
            Rating = 4,
            Comment = "Good product",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            IsDeleted = false,
            Product = _testProduct
        };
        _otherUserReview = new Review
        {
            Id = Guid.NewGuid(),
            UserId = _otherUserId,
            ProductId = _testProductId,
            Rating = 5,
            Comment = "Excellent!",
            CreatedAt = DateTime.UtcNow.AddDays(-2),
            IsDeleted = false,
            Product = _testProduct
        };
        _testReviews = new List<Review> { _existingReview, _otherUserReview };
        _testProducts = new List<Product> { _testProduct };
    }

    #region AddReviewAsync Tests

    [Test]
    public async Task AddReviewAsync_ShouldAddNewReview_WhenNoExistingReview()
    {
        var newRating = 5;
        var newComment = "Amazing product!";

        _reviewRepoMock.Setup(x => x.HasUserReviewedAsync(_testUserId, _testProductId))
            .ReturnsAsync(false);

        _reviewRepoMock.Setup(x => x.AddAsync(It.IsAny<Review>()))
            .Returns(Task.CompletedTask);

        _reviewRepoMock.Setup(x => x.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        await _reviewService.AddReviewAsync(_testUserId, _testProductId, newRating, newComment);

        _reviewRepoMock.Verify(r => r.AddAsync(It.Is<Review>(rev =>
            rev.UserId == _testUserId &&
            rev.ProductId == _testProductId &&
            rev.Rating == newRating &&
            rev.Comment == newComment)), Times.Once);

        _reviewRepoMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task AddReviewAsync_ShouldNotAddReview_WhenUserAlreadyReviewed()
    {
        _reviewRepoMock.Setup(x => x.HasUserReviewedAsync(_testUserId, _testProductId))
            .ReturnsAsync(true);

        var result = await _reviewService.AddReviewAsync(_testUserId, _testProductId, 5, "Great!");

        Assert.IsFalse(result);
        _reviewRepoMock.Verify(r => r.AddAsync(It.IsAny<Review>()), Times.Never);
        _reviewRepoMock.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    [Test]
    public async Task AddReviewAsync_ShouldReturnTrue_WhenReviewAddedSuccessfully()
    {
        _reviewRepoMock.Setup(x => x.HasUserReviewedAsync(_testUserId, _testProductId))
            .ReturnsAsync(false);

        _reviewRepoMock.Setup(x => x.AddAsync(It.IsAny<Review>()))
            .Returns(Task.CompletedTask);

        _reviewRepoMock.Setup(x => x.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        var result = await _reviewService.AddReviewAsync(_testUserId, _testProductId, 5, "Excellent!");

        Assert.IsTrue(result);

        _reviewRepoMock.Verify(r => r.AddAsync(It.IsAny<Review>()), Times.Once);
        _reviewRepoMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    #endregion AddReviewAsync Tests

    #region CreateReviewAsync Tests

    [Test]
    public async Task CreateReviewAsync_ShouldCreateReview_WhenNoExistingReview()
    {
        var model = new AddReviewViewModel
        {
            ProductId = _testProductId,
            Rating = 5,
            Comment = "Fantastic!"
        };

        _reviewRepoMock.Setup(x => x.HasUserReviewedAsync(_testUserId, _testProductId))
            .ReturnsAsync(false);

        _reviewRepoMock.Setup(x => x.AddAsync(It.IsAny<Review>()))
            .Returns(Task.CompletedTask);

        _reviewRepoMock.Setup(x => x.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        var result = await _reviewService.CreateReviewAsync(_testUserId, model);

        Assert.IsTrue(result.Success);

        _reviewRepoMock.Verify(r => r.AddAsync(It.Is<Review>(rev =>
            rev.UserId == _testUserId &&
            rev.ProductId == _testProductId &&
            rev.Rating == model.Rating &&
            rev.Comment == model.Comment)), Times.Once);

        _reviewRepoMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task CreateReviewAsync_ShouldNotCreateReview_WhenUserAlreadyReviewed()
    {
        var model = new AddReviewViewModel
        {
            ProductId = _testProductId,
            Rating = 5,
            Comment = "Fantastic!"
        };

        _reviewRepoMock.Setup(x => x.HasUserReviewedAsync(_testUserId, _testProductId))
            .ReturnsAsync(true);

        var result = await _reviewService.CreateReviewAsync(_testUserId, model);

        Assert.IsFalse(result.Success);

        Assert.AreEqual("You have already reviewed this product.", result.Error);

        _reviewRepoMock.Verify(r => r.AddAsync(It.IsAny<Review>()), Times.Never);
        _reviewRepoMock.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    [Test]
    public async Task CreateReviewAsync_ShouldReturnSuccess_WhenReviewCreated()
    {
        var model = new AddReviewViewModel
        {
            ProductId = _testProductId,
            Rating = 5,
            Comment = "Fantastic!"
        };
        _reviewRepoMock.Setup(x => x.HasUserReviewedAsync(_testUserId, _testProductId))
            .ReturnsAsync(false);
        _reviewRepoMock.Setup(x => x.AddAsync(It.IsAny<Review>()))
            .Returns(Task.CompletedTask);
        _reviewRepoMock.Setup(x => x.SaveChangesAsync())
            .Returns(Task.CompletedTask);
        var result = await _reviewService.CreateReviewAsync(_testUserId, model);

        Assert.IsTrue(result.Success);

        _reviewRepoMock.Verify(r => r.AddAsync(It.IsAny<Review>()), Times.Once);
        _reviewRepoMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    #endregion CreateReviewAsync Tests

    #region HasUserReviewedAsync Tests

    [Test]
    public async Task HasUserReviewedAsync_ShouldReturnTrue_WhenUserHasReviewed()
    {
        _reviewRepoMock.Setup(x => x.HasUserReviewedAsync(_testUserId, _testProductId))
            .ReturnsAsync(true);

        var result = await _reviewService.HasUserReviewedAsync(_testUserId, _testProductId);

        Assert.IsTrue(result);
    }

    [Test]
    public async Task HasUserReviewedAsync_ShouldReturnFalse_WhenUserHasNotReviewed()
    {
        _reviewRepoMock.Setup(x => x.HasUserReviewedAsync(_testUserId, _testProductId))
            .ReturnsAsync(false);

        var result = await _reviewService.HasUserReviewedAsync(_testUserId, _testProductId);

        Assert.IsFalse(result);
    }

    [Test]
    public async Task HasUserReviewedAsync_ShouldCallRepositoryMethod()
    {
        _reviewRepoMock.Setup(x => x.HasUserReviewedAsync(_testUserId, _testProductId))
            .ReturnsAsync(true);

        await _reviewService.HasUserReviewedAsync(_testUserId, _testProductId);

        _reviewRepoMock.Verify(x => x.HasUserReviewedAsync(_testUserId, _testProductId), Times.Once);
    }

    #endregion HasUserReviewedAsync Tests

    #region GetReviewsByProductIdAsync Tests

    public async Task GetReviewsByProductIdAsync_ShouldReturnReviews_WhenReviewsExist()
    {
        _reviewRepoMock.Setup(x => x.GetReviewsByProductIdAsync(_testProductId))
            .ReturnsAsync(_testReviews);

        var result = await _reviewService.GetReviewsByProductIdAsync(_testProductId);

        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count());
    }

    [Test]
    public async Task GetReviewsByProductIdAsync_ShouldReturnEmpty_WhenNoReviewsExist()
    {
        _reviewRepoMock.Setup(x => x.GetReviewsByProductIdAsync(_testProductId))
            .ReturnsAsync(new List<Review>());

        var result = await _reviewService.GetReviewsByProductIdAsync(_testProductId);

        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count());
    }

    [Test]
    public async Task GetReviewsByProductIdAsync_ShouldCallRepositoryMethod()
    {
        _reviewRepoMock.Setup(x => x.GetReviewsByProductIdAsync(_testProductId))
            .ReturnsAsync(_testReviews);

        await _reviewService.GetReviewsByProductIdAsync(_testProductId);

        _reviewRepoMock.Verify(x => x.GetReviewsByProductIdAsync(_testProductId), Times.Once);
    }

    #endregion GetReviewsByProductIdAsync Tests

    #region GetWriteReviewModelAsync Tests

    [Test]
    public async Task GetWriteReviewModelAsync_ShouldReturnProductViewModel_WhenUserHasNotReviewed()
    {
        _reviewRepoMock.Setup(x => x.HasUserReviewedAsync(_testUserId, _testProductId))
            .ReturnsAsync(false);

        _productRepoMock.Setup(x => x.GetProductWithReviewsAsync(_testProductId))
            .ReturnsAsync(_testProduct);

        var result = await _reviewService.GetWriteReviewModelAsync(_testUserId, _testProductId);

        Assert.IsNotNull(result);
        Assert.AreEqual(_testProduct.Id, result.Id);
        Assert.AreEqual(_testProduct.Name, result.Name);
        Assert.AreEqual(_testProduct.Description, result.Description);
    }

    [Test]
    public async Task GetWriteReviewModelAsync_ShouldReturnNull_WhenUserHasReviewed()
    {
        _reviewRepoMock.Setup(x => x.HasUserReviewedAsync(_testUserId, _testProductId))
            .ReturnsAsync(true);

        var result = await _reviewService.GetWriteReviewModelAsync(_testUserId, _testProductId);

        Assert.IsNull(result);
    }

    [Test]
    public async Task GetWriteReviewModelAsync_ShouldReturnNull_WhenProductNotFound()
    {
        _reviewRepoMock.Setup(x => x.HasUserReviewedAsync(_testUserId, _testProductId))
            .ReturnsAsync(false);

        _productRepoMock.Setup(x => x.GetProductWithReviewsAsync(_testProductId))
            .ReturnsAsync((Product)null);

        var result = await _reviewService.GetWriteReviewModelAsync(_testUserId, _testProductId);
        Assert.IsNull(result);
    }

    [Test]
    public async Task GetWriteReviewModelAsync_ShouldCallRepositoryMethods()
    {
        _reviewRepoMock.Setup(x => x.HasUserReviewedAsync(_testUserId, _testProductId))
            .ReturnsAsync(false);

        _productRepoMock.Setup(x => x.GetProductWithReviewsAsync(_testProductId))
            .ReturnsAsync(_testProduct);

        await _reviewService.GetWriteReviewModelAsync(_testUserId, _testProductId);

        _reviewRepoMock.Verify(x => x.HasUserReviewedAsync(_testUserId, _testProductId), Times.Once);

        _productRepoMock.Verify(x => x.GetProductWithReviewsAsync(_testProductId), Times.Once);
    }

    #endregion GetWriteReviewModelAsync Tests

    #region GetUserReviewsAsync Tests

    [Test]
    public async Task GetUserReviewsAsync_ShouldReturnUserReviews_WhenReviewsExist()
    {
        _reviewRepoMock.Setup(x => x.GetUserReviewsAsync(_testUserId))
            .ReturnsAsync(_testReviews);

        var result = await _reviewService.GetUserReviewsAsync(_testUserId);

        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count());

        _reviewRepoMock.Verify(x => x.GetUserReviewsAsync(_testUserId), Times.Once);
    }

    [Test]
    public async Task GetUserReviewsAsync_ShouldReturnEmpty_WhenNoReviewsExist()
    {
        _reviewRepoMock.Setup(x => x.GetUserReviewsAsync(_testUserId))
            .ReturnsAsync(new List<Review>());
        var result = await _reviewService.GetUserReviewsAsync(_testUserId);
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count());
        _reviewRepoMock.Verify(x => x.GetUserReviewsAsync(_testUserId), Times.Once);
    }

    [Test]
    public async Task GetUserReviewsAsync_ShouldCallRepositoryMethod()
    {
        _reviewRepoMock.Setup(x => x.GetUserReviewsAsync(_testUserId))
            .ReturnsAsync(_testReviews);

        await _reviewService.GetUserReviewsAsync(_testUserId);

        _reviewRepoMock.Verify(x => x.GetUserReviewsAsync(_testUserId), Times.Once);
    }

    #endregion GetUserReviewsAsync Tests
}