using GruersShop.Data.Models.Base;
using GruersShop.Data.Models.Interactions;
using GruersShop.Data.Models.Products;
using GruersShop.Data.Repositories.Interfaces.Bakery;
using GruersShop.Data.Repositories.Interfaces.Interactions;
using GruersShop.Services.Core.Service.Admin.Implementations.Interactions;
using GruersShop.Web.ViewModels.Interactions;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace GruersShop.Unit.Tests.Services.Admin.Interactions;

[TestFixture]
public class ReviewManagementServiceTests
{
    private Mock<IReviewManagementRepository> _reviewRepoMock;
    private Mock<IProductRepository> _productRepoMock;
    private ReviewManagementService _reviewService;

    private string _testUserId;
    private string _otherUserId;
    private Guid _testProductId;
    private Guid _otherProductId;
    private Review _existingReview;
    private Review _otherUserReview;
    private Review _deletedReview;
    private Review _otherDeletedReview;
    private Product _testProduct;
    private List<Review> _testReviews;
    private List<Review> _deletedReviews;
    private List<Product> _testProducts;

    [SetUp]
    public void SetUp()
    {
        _reviewRepoMock = new Mock<IReviewManagementRepository>(MockBehavior.Strict);
        _productRepoMock = new Mock<IProductRepository>(MockBehavior.Strict);
        _reviewService = new ReviewManagementService(_reviewRepoMock.Object, _productRepoMock.Object);
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
        _deletedReview = new Review
        {
            Id = Guid.NewGuid(),
            UserId = _testUserId,
            ProductId = _testProductId,
            Rating = 4,
            Comment = "Kinda good",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            IsDeleted = true,
            Product = _testProduct
        };
        _otherDeletedReview = new Review
        {
            Id = Guid.NewGuid(),
            UserId = _otherUserId,
            ProductId = _testProductId,
            Rating = 3,
            Comment = "Not so good",
            CreatedAt = DateTime.UtcNow.AddDays(-2),
            IsDeleted = true,
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
        _deletedReviews = new List<Review> { _deletedReview, _otherDeletedReview };
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

    #region GetAllActiveAsync Tests

    [Test]
    public async Task GetAllActiveAsync_ShouldReturnActiveReviews_WhenReviewsExist()
    {
        var mockDbSet = _testReviews.BuildMockDbSet();
        _reviewRepoMock.Setup(x => x.Query()).Returns(mockDbSet.Object);

        var result = await _reviewService.GetAllActiveAsync();
        var resultList = result.ToList();

        Assert.IsNotNull(result);
        Assert.AreEqual(2, resultList.Count);
        _reviewRepoMock.Verify(x => x.Query(), Times.Once);
    }

    [Test]
    public async Task GetAllActiveAsync_ShouldReturnEmpty_WhenNoActiveReviewsExist()
    {
        foreach (var review in _testReviews)
        {
            review.IsDeleted = true;
        }

        var mockDbSet = _testReviews.BuildMockDbSet();
        _reviewRepoMock.Setup(x => x.Query()).Returns(mockDbSet.Object);

        var result = await _reviewService.GetAllActiveAsync();
        var resultList = result.ToList();

        Assert.IsNotNull(result);
        Assert.AreEqual(0, resultList.Count);
        _reviewRepoMock.Verify(x => x.Query(), Times.Once);
    }

    [Test]
    public async Task GetAllActiveAsync_ShouldReturnEmpty_WhenNoReviewsExist()
    {
        var emptyList = new List<Review>();
        var mockDbSet = emptyList.BuildMockDbSet();
        _reviewRepoMock.Setup(x => x.Query()).Returns(mockDbSet.Object);

        var result = await _reviewService.GetAllActiveAsync();
        var resultList = result.ToList();

        Assert.IsNotNull(result);
        Assert.AreEqual(0, resultList.Count);
        _reviewRepoMock.Verify(x => x.Query(), Times.Once);
    }

    #endregion GetAllActiveAsync Tests

    #region GetAllIncludingDeletedAsync Tests

    [Test]
    public async Task GetAllIncludingDeletedAsync_ShouldReturnAllReviews_WhenReviewsExist()
    {
        var allReviews = _testReviews.Concat(_deletedReviews).ToList();
        var mockDbSet = allReviews.BuildMockDbSet();
        _reviewRepoMock.Setup(x => x.Query()).Returns(mockDbSet.Object);
        var result = await _reviewService.GetAllIncludingDeletedAsync();
        var resultList = result.ToList();
        Assert.IsNotNull(result);
        Assert.AreEqual(4, resultList.Count);
        _reviewRepoMock.Verify(x => x.Query(), Times.Once);
    }

    [Test]
    public async Task GetAllIncludingDeletedAsync_ShouldReturnEmpty_WhenNoReviewsExist()
    {
        var emptyList = new List<Review>();
        var mockDbSet = emptyList.BuildMockDbSet();
        _reviewRepoMock.Setup(x => x.Query()).Returns(mockDbSet.Object);
        var result = await _reviewService.GetAllIncludingDeletedAsync();
        var resultList = result.ToList();
        Assert.IsNotNull(result);
        Assert.AreEqual(0, resultList.Count);
        _reviewRepoMock.Verify(x => x.Query(), Times.Once);
    }

    [Test]
    public async Task GetAllIncludingDeletedAsync_ShouldReturnAllReviews_WhenBothActiveAndDeletedExist()
    {
        var allReviews = new List<Review>();
        allReviews.AddRange(_testReviews);
        allReviews.AddRange(_deletedReviews);

        var mockDbSet = allReviews.BuildMockDbSet();
        _reviewRepoMock.Setup(x => x.Query()).Returns(mockDbSet.Object);

        var result = await _reviewService.GetAllIncludingDeletedAsync();
        var resultList = result.ToList();

        Assert.IsNotNull(result);
        Assert.AreEqual(4, resultList.Count);
        _reviewRepoMock.Verify(x => x.Query(), Times.Once);
    }

    #endregion GetAllIncludingDeletedAsync Tests

    #region GetByIdAsync Tests

    [Test]
    public async Task GetByIdAsync_ShouldReturnReview_WhenReviewExists()
    {
        var reviewId = _existingReview.Id;
        var mockDbSet = _testReviews.BuildMockDbSet();
        _reviewRepoMock.Setup(x => x.Query()).Returns(mockDbSet.Object);
        var result = await _reviewService.GetByIdAsync(reviewId);
        Assert.IsNotNull(result);
        Assert.AreEqual(reviewId, result.Id);
        _reviewRepoMock.Verify(x => x.Query(), Times.Once);
    }

    [Test]
    public async Task GetByIdAsync_ShouldReturnNull_WhenReviewDoesNotExist()
    {
        var nonExistentId = Guid.NewGuid();
        var mockDbSet = _testReviews.BuildMockDbSet();

        _reviewRepoMock.Setup(x => x.Query()).Returns(mockDbSet.Object);
        var result = await _reviewService.GetByIdAsync(nonExistentId);
        Assert.IsNull(result);
        _reviewRepoMock.Verify(x => x.Query(), Times.Once);
    }

    [Test]
    public async Task GetByIdAsync_ShouldReturnReview_WhenReviewIsDeleted()
    {
        var reviewId = _deletedReview.Id;
        var allReviews = _testReviews.Concat(_deletedReviews).ToList();
        var mockDbSet = allReviews.BuildMockDbSet();
        _reviewRepoMock.Setup(x => x.Query()).Returns(mockDbSet.Object);
        var result = await _reviewService.GetByIdAsync(reviewId);
        Assert.IsNotNull(result);
        Assert.AreEqual(reviewId, result.Id);
        _reviewRepoMock.Verify(x => x.Query(), Times.Once);
    }

    #endregion GetByIdAsync Tests

    #region GetTotalActiveReviewsAsync Tests

    [Test]
    public async Task GetTotalActiveReviewsAsync_ShouldReturnCorrectCount_WhenActiveReviewsExist()
    {
        var mockDbSet = _testReviews.BuildMockDbSet();
        _reviewRepoMock.Setup(x => x.Query()).Returns(mockDbSet.Object);
        var result = await _reviewService.GetTotalActiveReviewsAsync();
        Assert.AreEqual(2, result);
        _reviewRepoMock.Verify(x => x.Query(), Times.Once);
    }

    [Test]
    public async Task GetTotalActiveReviewsAsync_ShouldReturnZero_WhenNoActiveReviewsExist()
    {
        foreach (var review in _testReviews)
        {
            review.IsDeleted = true;
        }
        var mockDbSet = _testReviews.BuildMockDbSet();
        _reviewRepoMock.Setup(x => x.Query()).Returns(mockDbSet.Object);
        var result = await _reviewService.GetTotalActiveReviewsAsync();
        Assert.AreEqual(0, result);
        _reviewRepoMock.Verify(x => x.Query(), Times.Once);
    }

    [Test]
    public async Task GetTotalActiveReviewsAsync_ShouldReturnZero_WhenNoReviewsExist()
    {
        var emptyList = new List<Review>();
        var mockDbSet = emptyList.BuildMockDbSet();
        _reviewRepoMock.Setup(x => x.Query()).Returns(mockDbSet.Object);
        var result = await _reviewService.GetTotalActiveReviewsAsync();
        Assert.AreEqual(0, result);
        _reviewRepoMock.Verify(x => x.Query(), Times.Once);
    }

    #endregion GetTotalActiveReviewsAsync Tests

    #region ToggleReviewAsync Tests

    [Test]
    public async Task ToggleReviewAsync_ShouldToggleIsDeleted_WhenActiveReviewExists()
    {
        var reviewId = _existingReview.Id;

        var reviewToToggle = new Review
        {
            Id = _existingReview.Id,
            IsDeleted = _existingReview.IsDeleted,
            UpdatedAt = _existingReview.UpdatedAt
        };

        var reviews = new List<Review> { reviewToToggle };
        var mockDbSet = reviews.BuildMockDbSet();

        _reviewRepoMock.Setup(x => x.Query()).Returns(mockDbSet.Object);

        Review capturedReview = null;
        _reviewRepoMock.Setup(x => x.UpdateAsync(It.IsAny<Review>()))
            .Callback<Review>(r => capturedReview = r)
            .ReturnsAsync(true);

        _reviewRepoMock.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);

        await _reviewService.ToggleReviewAsync(reviewId);

        Assert.That(capturedReview, Is.Not.Null);
        Assert.That(capturedReview.IsDeleted, Is.True);
    }

    [Test]
    public async Task ToggleReviewAsync_ShouldRestoreReview_WhenDeletedReviewExists()
    {
        var reviewId = _deletedReview.Id;

        var reviewToToggle = new Review
        {
            Id = _deletedReview.Id,
            IsDeleted = _deletedReview.IsDeleted,
            UpdatedAt = _deletedReview.UpdatedAt
        };

        var reviews = new List<Review> { reviewToToggle };
        var mockDbSet = reviews.BuildMockDbSet();

        _reviewRepoMock.Setup(x => x.Query()).Returns(mockDbSet.Object);

        Review capturedReview = null;
        _reviewRepoMock.Setup(x => x.UpdateAsync(It.IsAny<Review>()))
            .Callback<Review>(r => capturedReview = r)
            .ReturnsAsync(true);

        _reviewRepoMock.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);

        await _reviewService.ToggleReviewAsync(reviewId);

        Assert.That(capturedReview, Is.Not.Null);
        Assert.That(capturedReview.IsDeleted, Is.False);
    }

    [Test]
    public void ToggleReviewAsync_ShouldThrowException_WhenReviewNotFound()
    {
        var nonExistentId = Guid.NewGuid();

        var allReviews = _testReviews.Concat(_deletedReviews).ToList();
        var mockDbSet = allReviews.BuildMockDbSet();

        _reviewRepoMock.Setup(x => x.Query()).Returns(mockDbSet.Object);

        var ex = Assert.ThrowsAsync<Exception>(async () =>
            await _reviewService.ToggleReviewAsync(nonExistentId));

        Assert.That(ex.Message, Does.Contain("not found"));
    }

    #endregion ToggleReviewAsync Tests

    #region GetReviewsByUserIdAsync Tests

    [Test]
    public async Task GetReviewsByUserIdAsync_ShouldReturnUserReviews_WhenReviewsExist()
    {
        var _userReviews = new List<Review> { _testReviews[0], _testReviews[1] };
        var user = new AppUser { Id = _testUserId, UserName = "TestUser" };

        user.Reviews.Add(_userReviews[0]);
        user.Reviews.Add(_userReviews[1]);

        var mockDbSet = _userReviews.BuildMockDbSet();
        _reviewRepoMock.Setup(x => x.Query()).Returns(mockDbSet.Object);

        var result = await _reviewService.GetReviewsByUserIdAsync(_testUserId);

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count());

        Assert.IsTrue(result.All(r => r.UserId == _testUserId));
    }

    [Test]
    public async Task GetReviewsByUserIdAsync_ShouldReturnEmpty_WhenNoReviewsExist()
    {
        var emptyList = new List<Review>();
        var mockDbSet = emptyList.BuildMockDbSet();

        _reviewRepoMock.Setup(x => x.Query()).Returns(mockDbSet.Object);

        var result = await _reviewService.GetReviewsByUserIdAsync(_testUserId);

        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count());
    }

    [Test]
    public async Task GetReviewsByUserIdAsync_ShouldReturnEmpty_WhenUserHasNoReviews()
    {
        var otherUserReviews = new List<Review> { _otherUserReview };
        var mockDbSet = otherUserReviews.BuildMockDbSet();
        _reviewRepoMock.Setup(x => x.Query()).Returns(mockDbSet.Object);
        var result = await _reviewService.GetReviewsByUserIdAsync(_testUserId);
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count());
    }

    #endregion GetReviewsByUserIdAsync Tests

    #region GetDetailedReviewsByProductIdAsync Tests

    [Test]
    public async Task GetDetailedReviewsByProductIdAsync_ShouldReturnDetailedReviews_WhenReviewsExist()
    {
        var reviews = new List<Review> { _existingReview, _otherUserReview };

        _testProduct.Reviews.Add(_existingReview);
        _testProduct.Reviews.Add(_otherUserReview);

        var mockDbSet = reviews.BuildMockDbSet();
        _reviewRepoMock.Setup(x => x.Query()).Returns(mockDbSet.Object);

        var result = await _reviewService.GetDetailedReviewsByProductIdAsync(_testProductId);

        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count());
        Assert.That(_testProduct.Reviews, Has.All.Matches<Review>(r => r.ProductId == _testProductId));
    }

    [Test]
    public async Task GetDetailedReviewsByProductIdAsync_ShouldReturnEmpty_WhenNoReviewsExist()
    {
        var emptyList = new List<Review>();
        var mockDbSet = emptyList.BuildMockDbSet();
        _reviewRepoMock.Setup(x => x.Query()).Returns(mockDbSet.Object);
        var result = await _reviewService.GetDetailedReviewsByProductIdAsync(_testProductId);
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count());
    }

    [Test]
    public async Task GetDetailedReviewsByProductIdAsync_ShouldReturnEmpty_WhenNoReviewsForProductExist()
    {
        var otherProductReviews = new List<Review>();
        var mockDbSet = otherProductReviews.BuildMockDbSet();
        _reviewRepoMock.Setup(x => x.Query()).Returns(mockDbSet.Object);
        var result = await _reviewService.GetDetailedReviewsByProductIdAsync(_testProductId);
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count());
    }

    #endregion GetDetailedReviewsByProductIdAsync Tests

    #region GetAverageRatingForProductAsync Tests

    [Test]
    public async Task GetAverageRatingForProductAsync_ShouldReturnCorrectAverage_WhenReviewsExist()
    {
        var reviews = new List<Review> { _existingReview, _otherUserReview };
        var mockDbSet = reviews.BuildMockDbSet();
        _reviewRepoMock.Setup(x => x.Query()).Returns(mockDbSet.Object);
        var result = await _reviewService.GetAverageRatingForProductAsync(_testProductId);
        Assert.IsNotNull(result);
        Assert.AreEqual(4.5, result, 0.1);
    }

    [Test]
    public async Task GetAverageRatingForProductAsync_ShouldReturnZero_WhenNoReviewsExist()
    {
        var emptyList = new List<Review>();
        var mockDbSet = emptyList.BuildMockDbSet();
        _reviewRepoMock.Setup(x => x.Query()).Returns(mockDbSet.Object);
        var result = await _reviewService.GetAverageRatingForProductAsync(_testProductId);
        Assert.IsNotNull(result);
        Assert.AreEqual(0.0, result, 0.0);
    }

    #endregion GetAverageRatingForProductAsync Tests

    #region GetReviewCountForProductAsync Tests

    [Test]
    public async Task GetReviewCountForProductAsync_ShouldReturnCorrectCount_WhenReviewsExist()
    {
        var reviews = new List<Review> { _existingReview, _otherUserReview };
        var mockDbSet = reviews.BuildMockDbSet();
        _reviewRepoMock.Setup(x => x.Query()).Returns(mockDbSet.Object);
        var result = await _reviewService.GetReviewCountForProductAsync(_testProductId);
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result);
    }

    [Test]
    public async Task GetReviewCountForProductAsync_ShouldReturnZero_WhenNoReviewsExist()
    {
        var emptyList = new List<Review>();
        var mockDbSet = emptyList.BuildMockDbSet();
        _reviewRepoMock.Setup(x => x.Query()).Returns(mockDbSet.Object);
        var result = await _reviewService.GetReviewCountForProductAsync(_testProductId);
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result);
    }

    #endregion GetReviewCountForProductAsync Tests
}