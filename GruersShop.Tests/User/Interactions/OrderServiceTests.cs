using GruersShop.Data.Common.Enums;
using GruersShop.Data.Models.Catalog;
using GruersShop.Data.Models.Interactions;
using GruersShop.Data.Models.Products;
using GruersShop.Data.Repositories.Interfaces.Bakery;
using GruersShop.Data.Repositories.Interfaces.Interactions;
using GruersShop.Services.Core.Service.Implementations.Interactions;
using GruersShop.Services.Core.Service.Interfaces.Interactions;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace GruersShop.Unit.Tests.Services.User.Interactions;

[TestFixture]
public class OrderServiceTests
{
    private Mock<IOrderRepository> _orderRepoMock;
    private Mock<IProductRepository> _productRepoMock;
    private OrderService _orderService;

    private string _testUserId;
    private string _otherUserId;
    private Guid _testProductId;
    private Guid _otherProductId;
    private Order _existingOrder;
    private Order _otherUserOrder;
    private Product _testProduct;

    private Product _testProduct2;

    private List<Order> _testOrders;
    private List<Product> _testProducts;

    [SetUp]
    public void SetUp()
    {
        _orderRepoMock = new Mock<IOrderRepository>(MockBehavior.Strict);
        _productRepoMock = new Mock<IProductRepository>(MockBehavior.Strict);
        _orderService = new OrderService(_orderRepoMock.Object, _productRepoMock.Object);
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

        _testProduct2 = new Product
        {
            Id = _otherProductId,
            Name = "Other Product",
            Description = "Another product for testing",
            Price = 19.99m,
            StockQuantity = 100,
            IsDeleted = false,
            IsAvailable = true,
            CategoryId = testCategory.Id,
            Category = testCategory,
            AverageRating = 4.5,
            Reviews = new List<Review>()
        };

        _existingOrder = new Order()
        {
            Id = Guid.NewGuid(),
            UserId = _testUserId,
            OrderDate = DateTime.UtcNow.AddDays(-1),
            Status = OrderStatus.Pending,
            SpecialInstructions = "Leave at the door",
            TotalAmount = 19.98m,
            OrderItems = new List<OrderItem>()
        };

        _otherUserOrder = new Order
        {
            Id = Guid.NewGuid(),
            UserId = _otherUserId,
            OrderDate = DateTime.UtcNow.AddDays(-2),
            Status = OrderStatus.Pending,
            SpecialInstructions = "Will call upon arrival",
            TotalAmount = 20.00m,
            OrderItems = new List<OrderItem>()
        };

        _testOrders = new List<Order> { _existingOrder, _otherUserOrder };
        _testProducts = new List<Product> { _testProduct };
    }

    #region GetOrderByIdAsync Tests

    [Test]
    public async Task GetOrderByIdAsync_ReturnsOrder_WhenOrderExists()
    {
        _orderRepoMock.Setup(repo => repo.GetByIdAsync(_existingOrder.Id))
            .ReturnsAsync(_existingOrder);

        var result = await _orderService.GetOrderByIdAsync(_existingOrder.Id);

        Assert.IsNotNull(result);
        Assert.AreEqual(_existingOrder.Id, result.Id);
        Assert.AreEqual(_existingOrder.UserId, result.UserId);
    }

    [Test]
    public async Task GetOrderByIdAsync_ReturnsNull_WhenOrderDoesNotExist()
    {
        var nonExistentOrderId = Guid.NewGuid();
        _orderRepoMock.Setup(repo => repo.GetByIdAsync(nonExistentOrderId))
            .ReturnsAsync((Order?)null);

        var result = await _orderService.GetOrderByIdAsync(nonExistentOrderId);

        Assert.IsNull(result);
    }

    #endregion GetOrderByIdAsync Tests

    #region GetUserOrdersAsync Tests

    [Test]
    public async Task GetUserOrdersAsync_ReturnsOrders_ForGivenUserId()
    {
        var mockDbSet = _testOrders.BuildMockDbSet();

        _orderRepoMock.Setup(repo => repo.Query())
            .Returns(mockDbSet.Object);

        var result = await _orderService.GetUserOrdersAsync(_testUserId);
        var resultList = new List<Order>(result);

        Assert.That(resultList.Count, Is.EqualTo(1));
        Assert.That(resultList[0].Id, Is.EqualTo(_existingOrder.Id));
    }

    [Test]
    public async Task GetUserOrdersAsync_ReturnsEmpty_WhenNoOrdersForUser()
    {
        var mockDbSet = _testOrders.BuildMockDbSet();

        _orderRepoMock.Setup(repo => repo.Query())
            .Returns(mockDbSet.Object);

        var result = await _orderService.GetUserOrdersAsync("non-existent-user");

        Assert.IsNotNull(result);
        Assert.IsEmpty(result);
    }

    [Test]
    public async Task GetUserOrdersAsync_DoesNotReturnDeletedOrders()
    {
        _existingOrder.IsDeleted = true;

        var mockDbSet = _testOrders.BuildMockDbSet();

        _orderRepoMock.Setup(repo => repo.Query())
            .Returns(mockDbSet.Object);

        var result = await _orderService.GetUserOrdersAsync(_testUserId);

        Assert.IsNotNull(result);
        Assert.IsEmpty(result);
    }

    [Test]
    public async Task GetUserOrdersAsync_ReturnsMultipleOrders_ForGivenUserId()
    {
        var secondOrder = new Order
        {
            Id = Guid.NewGuid(),
            UserId = _testUserId,
            OrderDate = DateTime.UtcNow,
            Status = OrderStatus.Pending,
            SpecialInstructions = "Ring the bell",
            TotalAmount = 15.00m,
            OrderItems = new List<OrderItem>()
        };
        _testOrders.Add(secondOrder);

        var mockDbSet = _testOrders.BuildMockDbSet();

        _orderRepoMock.Setup(repo => repo.Query())
            .Returns(mockDbSet.Object);

        var result = await _orderService.GetUserOrdersAsync(_testUserId);
        var resultList = new List<Order>(result);

        Assert.That(resultList.Count, Is.EqualTo(2));
        Assert.That(resultList[0].UserId, Is.EqualTo(_testUserId));
        Assert.That(resultList[1].UserId, Is.EqualTo(_testUserId));
    }

    #endregion GetUserOrdersAsync Tests



    #region GetOrderWithItemsAsync Tests

    [Test]
    public async Task GetOrderWithItemsAsync_ReturnsOrderWithItems_WhenOrderExists()
    {
        var orderItem = new OrderItem
        {
            Id = Guid.NewGuid(),
            ProductId = _testProductId,
            Quantity = 2,
            UnitPrice = _testProduct.Price
        };
        var mockDbSet = _testOrders.BuildMockDbSet();

        _existingOrder.OrderItems.Add(orderItem);
        _orderRepoMock.Setup(repo => repo.Query())
                .Returns(mockDbSet.Object);

        var result = await _orderService.GetOrderWithItemsAsync(_existingOrder.Id);
        var resultList = new List<OrderItem>(result.OrderItems);

        Assert.IsNotNull(result);
        Assert.AreEqual(_existingOrder.Id, result.Id);
        Assert.AreEqual(1, resultList.Count);
        Assert.AreEqual(orderItem.Id, resultList[0].Id);
        Assert.AreEqual(orderItem.ProductId, resultList[0].ProductId);
        Assert.AreEqual(orderItem.Quantity, resultList[0].Quantity);
        Assert.AreEqual(orderItem.UnitPrice, resultList[0].UnitPrice);
    }

    [Test]
    public async Task GetOrderWithItemsAsync_ReturnsNull_WhenOrderDoesNotExist()
    {
        var nonExistentOrderId = Guid.NewGuid();
        var mockDbSet = _testOrders.BuildMockDbSet();

        _orderRepoMock.Setup(repo => repo.Query())
            .Returns(mockDbSet.Object);

        var result = await _orderService.GetOrderWithItemsAsync(nonExistentOrderId);

        Assert.IsNull(result);
    }

    [Test]
    public async Task GetOrderWithItemsAsync_DoesNotReturnDeletedOrder()
    {
        _existingOrder.IsDeleted = true;
        var mockDbSet = _testOrders.BuildMockDbSet();

        _orderRepoMock.Setup(repo => repo.Query())
            .Returns(mockDbSet.Object);

        var result = await _orderService.GetOrderWithItemsAsync(_existingOrder.Id);

        Assert.IsNull(result);
    }

    #endregion GetOrderWithItemsAsync Tests

    #region UpdateOrderStatusAsync Tests

    [Test]
    public async Task UpdateOrderStatusAsync_UpdatesStatus_WhenOrderExists()
    {
        var newStatus = OrderStatus.Completed;
        var orderId = _existingOrder.Id;

        _orderRepoMock.Setup(repo => repo.UpdateStatusAsync(orderId, newStatus))
            .Returns(Task.CompletedTask);

        await _orderService.UpdateOrderStatusAsync(orderId, newStatus);

        _orderRepoMock.Verify(repo => repo.UpdateStatusAsync(orderId, newStatus), Times.Once);
    }

    [Test]
    public async Task UpdateOrderStatusAsync_WhenCalled_CallsRepositoryMethod()
    {
        var newStatus = OrderStatus.Baking;
        var orderId = _existingOrder.Id;

        _orderRepoMock.Setup(repo => repo.UpdateStatusAsync(orderId, newStatus))
            .Returns(Task.CompletedTask);

        await _orderService.UpdateOrderStatusAsync(orderId, newStatus);

        _orderRepoMock.Verify(repo => repo.UpdateStatusAsync(
            It.Is<Guid>(id => id == orderId),
            It.Is<OrderStatus>(status => status == newStatus)), Times.Once);
    }

    [Test]
    public async Task UpdateOrderStatusAsync_DoesNotThrow_WhenOrderDoesNotExist()
    {
        var nonExistentOrderId = Guid.NewGuid();
        var newStatus = OrderStatus.Cancelled;

        _orderRepoMock.Setup(repo => repo.UpdateStatusAsync(nonExistentOrderId, newStatus))
            .Returns(Task.CompletedTask);

        Assert.DoesNotThrowAsync(() => _orderService.UpdateOrderStatusAsync(nonExistentOrderId, newStatus));

        _orderRepoMock.Verify(repo => repo.UpdateStatusAsync(nonExistentOrderId, newStatus), Times.Once);
    }

    #endregion UpdateOrderStatusAsync Tests

    #region CancelOrderAsync Tests

    [Test]
    public void CancelOrderAsync_ThrowsException_WhenOrderNotFound()
    {

        var nonExistentOrderId = Guid.NewGuid();
        var emptyList = new List<Order>();
        var mockDbSet = emptyList.BuildMockDbSet();

        _orderRepoMock.Setup(repo => repo.Query())
            .Returns(mockDbSet.Object);


        var ex = Assert.ThrowsAsync<Exception>(async () =>
            await _orderService.CancelOrderAsync(nonExistentOrderId));

        Assert.That(ex.Message, Is.EqualTo("Order cannot be cancelled"));
    }

    [Test]
    public async Task CancelOrderAsync_UpdatesStatusToCancelled_WhenOrderExists()
    {
        var orderId = _existingOrder.Id;

        _existingOrder.Status = OrderStatus.Pending;
        _existingOrder.OrderItems = new List<OrderItem>
    {
        new OrderItem { ProductId = _testProductId, Quantity = 2 }
    };

        _orderRepoMock.Setup(repo => repo.GetOrderWithProductsAsync(orderId))
            .ReturnsAsync(_existingOrder);

        _orderRepoMock.Setup(repo => repo.UpdateStatusAsync(orderId, OrderStatus.Cancelled))
            .Returns(Task.CompletedTask);

        _productRepoMock.Setup(repo => repo.GetByIdAsync(_testProductId))
            .ReturnsAsync(_testProduct);
        _productRepoMock.Setup(repo => repo.UpdateAsync(It.IsAny<Product>()))
            .ReturnsAsync(true);

        _orderRepoMock.Setup(repo => repo.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        await _orderService.CancelOrderAsync(orderId);

        _orderRepoMock.Verify(repo => repo.UpdateStatusAsync(orderId, OrderStatus.Cancelled), Times.Once);
        _orderRepoMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
    }



    [Test]
    public void CancelOrderAsync_ThrowsException_WhenOrderIsNotPending()
    {
        var orderId = _existingOrder.Id;

        _existingOrder.Status = OrderStatus.Completed;

        _orderRepoMock.Setup(repo => repo.GetOrderWithProductsAsync(orderId))
            .ReturnsAsync(_existingOrder);

        var ex = Assert.ThrowsAsync<Exception>(async () =>
            await _orderService.CancelOrderAsync(orderId));

        Assert.That(ex.Message, Is.EqualTo("Order cannot be cancelled"));
    }

    [Test]
    public async Task CancelOrderAsync_RestoresStock_WhenOrderIsCancelled()
    {
        var orderId = _existingOrder.Id;
        var initialStock = _testProduct.StockQuantity;
        var orderedQuantity = 2;

        _existingOrder.Status = OrderStatus.Pending;
        _existingOrder.OrderItems = new List<OrderItem>
    {
        new OrderItem { ProductId = _testProductId, Quantity = orderedQuantity }
    };

        _orderRepoMock.Setup(repo => repo.GetOrderWithProductsAsync(orderId))
            .ReturnsAsync(_existingOrder);

        _orderRepoMock.Setup(repo => repo.UpdateStatusAsync(orderId, OrderStatus.Cancelled))
            .Returns(Task.CompletedTask);

        _orderRepoMock.Setup(repo => repo.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        _productRepoMock.Setup(repo => repo.GetByIdAsync(_testProductId))
            .ReturnsAsync(_testProduct);

        _productRepoMock.Setup(repo => repo.UpdateAsync(It.IsAny<Product>()))
            .ReturnsAsync(true);

        await _orderService.CancelOrderAsync(orderId);

        _productRepoMock.Verify(repo => repo.UpdateAsync(It.Is<Product>(p =>
            p.StockQuantity == initialStock + orderedQuantity)), Times.Once);
    }

    #endregion CancelOrderAsync Tests
}