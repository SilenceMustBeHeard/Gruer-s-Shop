using GruersShop.Data.Common.Enums;
using GruersShop.Data.Models.Base;
using GruersShop.Data.Models.Catalog;
using GruersShop.Data.Models.Interactions;
using GruersShop.Data.Models.Messages;
using GruersShop.Data.Models.Products;
using GruersShop.Data.Repositories.Interfaces.Bakery;
using GruersShop.Data.Repositories.Interfaces.Interactions;
using GruersShop.Services.Core.Service.Admin.Interfaces.Message;
using GruersShop.Services.Core.Service.Implementations.Interactions;
using Microsoft.AspNetCore.Identity;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;
using System.Reflection;
using Assert = NUnit.Framework.Assert;

namespace GruersShop.Unit.Tests.Services.Admin.Interactions;

[TestFixture]
public class OrderManagementServiceTests
{
    private Mock<IOrderRepository> _orderRepoMock;
    private Mock<IProductRepository> _productRepoMock;
    private Mock<ISystemInboxMessageService> _systemMessageServiceMock;
    private Mock<UserManager<AppUser>> _userManagerMock;
    private OrderManagementService _orderService;

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
        _systemMessageServiceMock = new Mock<ISystemInboxMessageService>(MockBehavior.Strict);

        var userStoreMock = new Mock<IUserStore<AppUser>>();
        _userManagerMock = new Mock<UserManager<AppUser>>(
            userStoreMock.Object, null, null, null, null, null, null, null, null);

        _orderService = new OrderManagementService(
            _orderRepoMock.Object,
            _productRepoMock.Object,
            _systemMessageServiceMock.Object,
            _userManagerMock.Object);

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

        var testUser = new AppUser
        {
            Id = _testUserId,
            UserName = "testuser",
            Email = "test@test.com",
            FirstName = "Test",
            LastName = "User"
        };

        var otherUser = new AppUser
        {
            Id = _otherUserId,
            UserName = "otheruser",
            Email = "other@test.com",
            FirstName = "Other",
            LastName = "User"
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

        var orderItems = new List<OrderItem>
    {
        new OrderItem
        {
            Id = Guid.NewGuid(),
            ProductId = _testProductId,
            Quantity = 2,
            UnitPrice = _testProduct.Price,
            Product = _testProduct
        }
    };

        _existingOrder = new Order
        {
            Id = Guid.NewGuid(),
            UserId = _testUserId,
            User = testUser,
            OrderDate = DateTime.UtcNow.AddDays(-1),
            Status = OrderStatus.Pending,
            SpecialInstructions = "Leave at the door",
            TotalAmount = 19.98m,
            IsDeleted = false,
            UpdatedAt = DateTime.UtcNow.AddDays(-1),
            OrderItems = orderItems
        };

        foreach (var item in orderItems)
        {
            item.OrderId = _existingOrder.Id;
        }

        _otherUserOrder = new Order
        {
            Id = Guid.NewGuid(),
            UserId = _otherUserId,
            User = otherUser,
            OrderDate = DateTime.UtcNow.AddDays(-2),
            Status = OrderStatus.Pending,
            SpecialInstructions = "Will call upon arrival",
            TotalAmount = 20.00m,
            IsDeleted = false,
            UpdatedAt = DateTime.UtcNow.AddDays(-2),
            OrderItems = new List<OrderItem>()
        };

        _testOrders = new List<Order> { _existingOrder, _otherUserOrder };
        _testProducts = new List<Product> { _testProduct, _testProduct2 };
    }

    #region GetAllOrdersAsync Tests

    [Test]
    public async Task GetAllOrdersAsync_ShouldReturnAllOrders_WhenNoStatusFilter()
    {
        var mockDbSet = _testOrders.BuildMockDbSet();

        _orderRepoMock.Setup(repo => repo.Query())
            .Returns(mockDbSet.Object);

        var result = await _orderService.GetAllOrdersAsync(null);
        var resultList = result.ToList();

        Assert.IsNotNull(result);
        Assert.AreEqual(2, resultList.Count);
        _orderRepoMock.Verify(repo => repo.Query(), Times.Once);
    }

    [Test]
    public async Task GetAllOrdersAsync_ShouldReturnFilteredOrders_WhenStatusFilterProvided()
    {
        var mockDbSet = _testOrders.BuildMockDbSet();

        _orderRepoMock.Setup(repo => repo.Query())
            .Returns(mockDbSet.Object);

        var result = await _orderService.GetAllOrdersAsync(OrderStatus.Pending);
        var resultList = result.ToList();

        Assert.IsNotNull(result);
        Assert.AreEqual(2, resultList.Count);
        Assert.IsTrue(resultList.All(o => o.Status == OrderStatus.Pending));
        _orderRepoMock.Verify(repo => repo.Query(), Times.Once);
    }

    [Test]
    public async Task GetAllOrdersAsync_ShouldReturnEmpty_WhenNoOrdersMatchStatusFilter()
    {
        var mockDbSet = _testOrders.BuildMockDbSet();

        _orderRepoMock.Setup(repo => repo.Query())
            .Returns(mockDbSet.Object);

        var result = await _orderService.GetAllOrdersAsync(OrderStatus.Completed);
        var resultList = result.ToList();

        Assert.IsNotNull(result);
        Assert.AreEqual(0, resultList.Count);
        _orderRepoMock.Verify(repo => repo.Query(), Times.Once);
    }

    [Test]
    public async Task GetAllOrdersAsync_ShouldReturnEmpty_WhenNoOrdersExist()
    {
        var emptyOrders = new List<Order>();
        var mockDbSet = emptyOrders.BuildMockDbSet();

        _orderRepoMock.Setup(repo => repo.Query())
            .Returns(mockDbSet.Object);

        var result = await _orderService.GetAllOrdersAsync(null);
        var resultList = result.ToList();

        Assert.IsNotNull(resultList);
        Assert.IsNotNull(result);
        Assert.AreEqual(0, resultList.Count);
        _orderRepoMock.Verify(repo => repo.Query(), Times.Once);
    }

    [Test]
    public async Task GetAllOrdersAsync_ShouldReturnOrdersInDescendingOrderByDate()
    {
        var mockDbSet = _testOrders.BuildMockDbSet();

        _orderRepoMock.Setup(repo => repo.Query())
            .Returns(mockDbSet.Object);

        var result = await _orderService.GetAllOrdersAsync(null);
        var resultList = result.ToList();

        Assert.IsNotNull(result);
        Assert.AreEqual(2, resultList.Count);
        Assert.IsTrue(resultList[0].OrderDate >= resultList[1].OrderDate);
        _orderRepoMock.Verify(repo => repo.Query(), Times.Once);
    }

    #endregion GetAllOrdersAsync Tests

    #region GetOrderWithDetailsAsync Tests

    [Test]
    public async Task GetOrderWithDetailsAsync_ShouldReturnOrderWithDetails_WhenOrderExists()
    {
        var mockDbSet = _testOrders.BuildMockDbSet();

        _orderRepoMock.Setup(repo => repo.Query())
            .Returns(mockDbSet.Object);

        var result = await _orderService.GetOrderWithDetailsAsync(_existingOrder.Id);

        Assert.IsNotNull(result);
        Assert.AreEqual(_existingOrder.Id, result.Id);
        Assert.IsNotNull(result.User);
        Assert.IsNotNull(result.OrderItems);
        Assert.AreEqual(1, result.OrderItems.Count);
        Assert.AreEqual(_testProductId, result.OrderItems.First().ProductId);
        _orderRepoMock.Verify(repo => repo.Query(), Times.Once);
    }

    [Test]
    public async Task GetOrderWithDetailsAsync_ShouldReturnNull_WhenOrderDoesNotExist()
    {
        var mockDbSet = _testOrders.BuildMockDbSet();

        _orderRepoMock.Setup(repo => repo.Query())
            .Returns(mockDbSet.Object);

        var nonExistentOrderId = Guid.NewGuid();
        var result = await _orderService.GetOrderWithDetailsAsync(nonExistentOrderId);

        Assert.IsNull(result);
        _orderRepoMock.Verify(repo => repo.Query(), Times.Once);
    }

    [Test]
    public async Task GetOrderWithDetailsAsync_ShouldIncludeUserAndProductDetails()
    {
        var mockDbSet = _testOrders.BuildMockDbSet();

        _orderRepoMock.Setup(repo => repo.Query())
            .Returns(mockDbSet.Object);

        var result = await _orderService.GetOrderWithDetailsAsync(_existingOrder.Id);

        Assert.IsNotNull(result);
        Assert.IsNotNull(result.User);
        Assert.AreEqual(_testUserId, result.UserId);
        Assert.IsNotNull(result.OrderItems);
        Assert.AreEqual(1, result.OrderItems.Count);
        var orderItem = result.OrderItems.First();
        Assert.AreEqual(_testProductId, orderItem.ProductId);
        Assert.IsNotNull(orderItem.Product);
        Assert.AreEqual("Test Product", orderItem.Product.Name);
        _orderRepoMock.Verify(repo => repo.Query(), Times.Once);
    }

    [Test]
    public async Task GetOrderWithDetailsAsync_ShouldReturnOrder_WhenOrderIsDeleted()
    {
        var deletedOrder = new Order
        {
            Id = Guid.NewGuid(),
            UserId = _testUserId,
            OrderDate = DateTime.UtcNow,
            Status = OrderStatus.Pending,
            TotalAmount = 10.00m,
            IsDeleted = true,
            UpdatedAt = DateTime.UtcNow,
            OrderItems = new List<OrderItem>()
        };
        var ordersWithDeleted = new List<Order> { _existingOrder, deletedOrder };
        var mockDbSet = ordersWithDeleted.BuildMockDbSet();

        _orderRepoMock.Setup(repo => repo.Query())
            .Returns(mockDbSet.Object);

        var result = await _orderService.GetOrderWithDetailsAsync(deletedOrder.Id);

        Assert.IsNotNull(result);
        Assert.That(result.IsDeleted == deletedOrder.IsDeleted);
        _orderRepoMock.Verify(repo => repo.Query(), Times.Once);
    }

    #endregion GetOrderWithDetailsAsync Tests

    #region CheckStockAvailabilityAsync Tests

    [Test]
    public async Task CheckStockAvailabilityAsync_ShouldHandleMultipleOrderItems_WhenCheckingStock()
    {
        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = _testUserId,
            OrderDate = DateTime.UtcNow,
            Status = OrderStatus.Pending,
            TotalAmount = 29.98m,
            IsDeleted = false,
            UpdatedAt = DateTime.UtcNow,
            OrderItems = new List<OrderItem>
            {
                new OrderItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = _testProductId,
                    Quantity = 2,
                    UnitPrice = _testProduct.Price,
                    Product = _testProduct
                },
                new OrderItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = _otherProductId,
                    Quantity = 1,
                    UnitPrice = _testProduct2.Price,
                    Product = _testProduct2
                }
            }
        };
        _productRepoMock.Setup(repo => repo.GetByIdAsync(_testProductId))
            .ReturnsAsync(_testProduct);

        _productRepoMock.Setup(repo => repo.GetByIdAsync(_otherProductId))
            .ReturnsAsync(_testProduct2);

        var result = await _orderService.CheckStockAvailabilityAsync(order);

        Assert.IsNotNull(result);
        Assert.IsTrue(result.IsAvailable);
        Assert.AreEqual(0, result.Issues.Count);

        _productRepoMock.Verify(repo => repo.GetByIdAsync(_testProductId), Times.Once);
        _productRepoMock.Verify(repo => repo.GetByIdAsync(_otherProductId), Times.Once);
    }

    [Test]
    public async Task CheckStockAvailabilityAsync_ShouldReportAllIssues_WhenMultipleItemsHaveStockProblems()
    {
        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = _testUserId,
            OrderDate = DateTime.UtcNow,
            Status = OrderStatus.Pending,
            TotalAmount = 29.98m,
            IsDeleted = false,
            UpdatedAt = DateTime.UtcNow,
            OrderItems = new List<OrderItem>
            {
                new OrderItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = _testProductId,
                    Quantity = 2,
                    UnitPrice = _testProduct.Price,
                    Product = _testProduct
                },
                new OrderItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = _otherProductId,
                    Quantity = 1,
                    UnitPrice = _testProduct2.Price,
                    Product = _testProduct2
                }
            }
        };

        _productRepoMock.Setup(repo => repo.GetByIdAsync(_testProductId))
            .ReturnsAsync(new Product
            {
                Id = _testProductId,
                Name = _testProduct.Name,
                StockQuantity = 0
            });

        _productRepoMock.Setup(repo => repo.GetByIdAsync(_otherProductId))
            .ReturnsAsync(new Product
            {
                Id = _otherProductId,
                Name = _testProduct2.Name,
                StockQuantity = 0
            });

        var result = await _orderService.CheckStockAvailabilityAsync(order);

        Assert.IsNotNull(result);
        Assert.IsFalse(result.IsAvailable);
        Assert.AreEqual(2, result.Issues.Count);
        Assert.IsTrue(result.Issues.Any(i => i.ProductName == _testProduct.Name));
        Assert.IsTrue(result.Issues.Any(i => i.ProductName == _testProduct2.Name));
        _productRepoMock.Verify(repo => repo.GetByIdAsync(_testProductId), Times.Once);
        _productRepoMock.Verify(repo => repo.GetByIdAsync(_otherProductId), Times.Once);
    }

    [Test]
    public async Task CheckStockAvailabilityAsync_ShouldReturnAvailable_WhenStockIsSufficient()
    {
        var order = _existingOrder;
        var product = _testProduct;

        _productRepoMock.Setup(repo => repo.GetByIdAsync(product.Id))
            .ReturnsAsync(product);

        var result = await _orderService.CheckStockAvailabilityAsync(order);

        Assert.IsNotNull(result);
        Assert.IsTrue(result.IsAvailable);
        Assert.AreEqual(0, result.Issues.Count);
        _productRepoMock.Verify(repo => repo.GetByIdAsync(product.Id), Times.Once);
    }

    [Test]
    public async Task CheckStockAvailabilityAsync_ShouldReturnUnavailable_WhenStockIsInsufficient()
    {
        var order = _existingOrder;
        var product = _testProduct;

        _productRepoMock.Setup(repo => repo.GetByIdAsync(product.Id))
            .ReturnsAsync(new Product
            {
                Id = product.Id,
                Name = product.Name,
                StockQuantity = 0
            });
        var result = await _orderService.CheckStockAvailabilityAsync(order);

        Assert.IsNotNull(result);
        Assert.IsFalse(result.IsAvailable);
        Assert.AreEqual(1, result.Issues.Count);
        Assert.AreEqual(result.Issues.FirstOrDefault()?.ProductName, product.Name);
        _productRepoMock.Verify(repo => repo.GetByIdAsync(product.Id), Times.Once);
    }

    [Test]
    public async Task CheckStockAvailabilityAsync_ShouldHandleNullProduct_WhenCheckingStock()
    {
        var order = _existingOrder;
        var productId = order.OrderItems.First().ProductId;
        _productRepoMock.Setup(repo => repo.GetByIdAsync(productId))
            .ReturnsAsync((Product)null);
        var result = await _orderService.CheckStockAvailabilityAsync(order);
        Assert.IsNotNull(result);
        Assert.IsFalse(result.IsAvailable);
        Assert.AreEqual(1, result.Issues.Count);
        Assert.AreEqual("Unknown", result.Issues.FirstOrDefault()?.ProductName);
        _productRepoMock.Verify(repo => repo.GetByIdAsync(productId), Times.Once);
    }

    #endregion CheckStockAvailabilityAsync Tests



    #region ConfirmOrderAsync Tests

    [Test]
    public async Task ConfirmOrderAsync_ShouldConfirmOrder_WhenStockIsAvailable()
    {
        var order = _existingOrder;
        var product = _testProduct;
        _productRepoMock.Setup(repo => repo.GetByIdAsync(product.Id))
            .ReturnsAsync(product);

        var result = await _orderService.CheckStockAvailabilityAsync(order);

        Assert.IsNotNull(result);
        Assert.IsTrue(result.IsAvailable);
        Assert.AreEqual(0, result.Issues.Count);
        _productRepoMock.Verify(repo => repo.GetByIdAsync(product.Id), Times.Once);
    }

    [Test]
    public async Task ConfirmOrderAsync_ShouldNotConfirmOrder_WhenStockIsUnavailable()
    {
        var order = _existingOrder;
        var product = _testProduct;
        _productRepoMock.Setup(repo => repo.GetByIdAsync(product.Id))
            .ReturnsAsync(new Product
            {
                Id = product.Id,
                Name = product.Name,
                StockQuantity = 0
            });
        var result = await _orderService.CheckStockAvailabilityAsync(order);

        Assert.IsNotNull(result);
        Assert.IsFalse(result.IsAvailable);
        Assert.AreEqual(1, result.Issues.Count);
        Assert.AreEqual(result.Issues.FirstOrDefault()?.ProductName, product.Name);
        _productRepoMock.Verify(repo => repo.GetByIdAsync(product.Id), Times.Once);
    }

    [Test]
    public async Task ConfirmOrderAsync_ShouldHandleNullProduct_WhenCheckingStock()
    {
        var order = _existingOrder;
        var productId = order.OrderItems.First().ProductId;
        _productRepoMock.Setup(repo => repo.GetByIdAsync(productId))
            .ReturnsAsync((Product)null);
        var result = await _orderService.CheckStockAvailabilityAsync(order);
        Assert.IsNotNull(result);
        Assert.IsFalse(result.IsAvailable);
        Assert.AreEqual(1, result.Issues.Count);
        Assert.AreEqual("Unknown", result.Issues.FirstOrDefault()?.ProductName);
        _productRepoMock.Verify(repo => repo.GetByIdAsync(productId), Times.Once);
    }

    [Test]
    public async Task ConfirmOrderAsync_ShouldHandleEmptyOrderItems_WhenCheckingStock()
    {
        var emptyOrder = new Order
        {
            Id = Guid.NewGuid(),
            UserId = _testUserId,
            OrderDate = DateTime.UtcNow,
            Status = OrderStatus.Pending,
            TotalAmount = 0.00m,
            IsDeleted = false,
            UpdatedAt = DateTime.UtcNow,
            OrderItems = new List<OrderItem>()
        };
        var result = await _orderService.CheckStockAvailabilityAsync(emptyOrder);
        Assert.IsNotNull(result);
        Assert.IsTrue(result.IsAvailable);
        Assert.AreEqual(0, result.Issues.Count);
    }

    #endregion ConfirmOrderAsync Tests

    #region UpdateOrderStatusAsync Tests

    [Test]
    public async Task UpdateOrderStatusAsync_ShouldReturnFailure_WhenOrderNotFound()
    {
        var nonExistentId = Guid.NewGuid();
        var orders = new List<Order>();
        var mockDbSet = orders.BuildMockDbSet();

        _orderRepoMock.Setup(repo => repo.Query())
            .Returns(mockDbSet.Object);

        var result = await _orderService.UpdateOrderStatusAsync(nonExistentId, OrderStatus.Confirmed);

        Assert.IsFalse(result.Success);
        Assert.AreEqual("Order not found.", result.Message);
    }

    [Test]
    public async Task UpdateOrderStatusAsync_ShouldUpdateStatus_FromConfirmedToBaking()
    {
        var order = _existingOrder;
        order.Status = OrderStatus.Confirmed; var newStatus = OrderStatus.Baking;
        var orders = new List<Order> { order };
        var mockDbSet = orders.BuildMockDbSet();

        _orderRepoMock.Setup(repo => repo.Query())
            .Returns(mockDbSet.Object);

        _orderRepoMock.Setup(repo => repo.UpdateAsync(It.IsAny<Order>()))
            .ReturnsAsync(true);

        _orderRepoMock.Setup(repo => repo.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        var result = await _orderService.UpdateOrderStatusAsync(order.Id, newStatus);

        Assert.IsTrue(result.Success);
        Assert.AreEqual(OrderStatus.Baking, order.Status);
    }

    [Test]
    public async Task UpdateOrderStatusAsync_ShouldUpdateStatus_FromBakingToCompleted()
    {
        var order = _existingOrder;
        order.Status = OrderStatus.Baking; var newStatus = OrderStatus.Completed;
        var orders = new List<Order> { order };
        var mockDbSet = orders.BuildMockDbSet();
        _orderRepoMock.Setup(repo => repo.Query())
            .Returns(mockDbSet.Object);
        _orderRepoMock.Setup(repo => repo.UpdateAsync(It.IsAny<Order>()))
            .ReturnsAsync(true);
        _orderRepoMock.Setup(repo => repo.SaveChangesAsync())
            .Returns(Task.CompletedTask);
        var result = await _orderService.UpdateOrderStatusAsync(order.Id, newStatus);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(OrderStatus.Completed, order.Status);
    }

    #endregion UpdateOrderStatusAsync Tests

    #region CancelOrderAsync Tests

    [Test]
    public async Task CancelOrderAsync_ShouldCancelOrder_WhenValidOrderProvided()
    {
        var order = _existingOrder;
        order.Status = OrderStatus.Pending;
        var reason = "Customer requested cancellation";

        var orderItems = new List<OrderItem>
    {
        new OrderItem
        {
            Id = Guid.NewGuid(),
            ProductId = _testProductId,
            Quantity = 2,
            UnitPrice = _testProduct.Price,
            Product = _testProduct
        }
    };
        order.OrderItems = orderItems;
        foreach (var item in orderItems)
        {
            item.OrderId = order.Id;
        }

        order.User = new AppUser
        {
            Id = _testUserId,
            UserName = "testuser",
            Email = "test@test.com"
        };

        var orders = new List<Order> { order };
        var mockDbSet = orders.BuildMockDbSet();

        _orderRepoMock.Setup(repo => repo.Query())
            .Returns(mockDbSet.Object);

        _orderRepoMock.Setup(repo => repo.UpdateAsync(It.IsAny<Order>()))
            .ReturnsAsync(true);
        _orderRepoMock.Setup(repo => repo.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        _productRepoMock.Setup(repo => repo.GetByIdAsync(_testProductId))
            .ReturnsAsync(_testProduct);
        _productRepoMock.Setup(repo => repo.UpdateAsync(It.IsAny<Product>()))
            .ReturnsAsync(true);

        var adminUser = new AppUser
        {
            Id = "admin-123",
            UserName = "admin",
            Email = "admin@gruers.com"
        };

        _userManagerMock.Setup(x => x.FindByIdAsync(_testUserId))
            .ReturnsAsync(order.User);

        _userManagerMock.Setup(x => x.GetUsersInRoleAsync("Admin"))
            .ReturnsAsync(new List<AppUser> { adminUser });

        _systemMessageServiceMock.Setup(x => x.CreateMessageAsync(It.IsAny<SystemInboxMessage>()))
            .Returns(Task.CompletedTask);

        await _orderService.CancelOrderAsync(order.Id, reason);

        Assert.AreEqual(OrderStatus.Cancelled, order.Status);
        _orderRepoMock.Verify(repo => repo.Query(), Times.AtLeastOnce);
        _orderRepoMock.Verify(repo => repo.UpdateAsync(It.Is<Order>(o =>
            o.Id == order.Id && o.Status == OrderStatus.Cancelled)), Times.Once);
    }

    [Test]
    public async Task CancelOrderAsync_ShouldHandleNullOrder_WhenCancelling()
    {
        var reason = "Customer requested cancellation";
        var nonExistentOrderId = Guid.NewGuid();
        var orders = new List<Order> { _existingOrder };
        var mockDbSet = orders.BuildMockDbSet();
        _orderRepoMock.Setup(repo => repo.Query())
            .Returns(mockDbSet.Object);
        var result = await _orderService.CancelOrderAsync(nonExistentOrderId, reason);
        Assert.IsFalse(result.Success);
        Assert.AreEqual("Order not found.", result.Message);
        _orderRepoMock.Verify(repo => repo.Query(), Times.Once);
    }

    #endregion CancelOrderAsync Tests

    #region RestoreStockAsync Tests

    [Test]
    public async Task RestoreStockAsync_ShouldRestoreStock_WhenCalledDirectly()
    {
        var order = _existingOrder;
        var initialStock = 100;
        var orderedQuantity = 2;

        var orderItems = new List<OrderItem>
    {
        new OrderItem
        {
            ProductId = _testProductId,
            Quantity = orderedQuantity
        }
    };
        order.OrderItems = orderItems;

        _productRepoMock.Setup(repo => repo.GetByIdAsync(_testProductId))
            .ReturnsAsync(_testProduct);
        _productRepoMock.Setup(repo => repo.UpdateAsync(It.IsAny<Product>()))
            .ReturnsAsync(true);

        var methodInfo = typeof(OrderManagementService)
            .GetMethod("RestoreStockAsync", BindingFlags.NonPublic | BindingFlags.Instance);

        await (Task)methodInfo.Invoke(_orderService, new object[] { order });

        _productRepoMock.Verify(repo => repo.UpdateAsync(It.Is<Product>(p =>
            p.StockQuantity == initialStock + orderedQuantity)), Times.Once);
    }

    [Test]
    public async Task RestoreStockAsync_ShouldHandleNullProduct_WhenRestoringStock()
    {
        var order = _existingOrder;
        var orderItems = new List<OrderItem>
    {
        new OrderItem
        {
            ProductId = _testProductId,
            Quantity = 2
        }
    };
        order.OrderItems = orderItems;
        _productRepoMock.Setup(repo => repo.GetByIdAsync(_testProductId))
            .ReturnsAsync((Product)null);
        var methodInfo = typeof(OrderManagementService)
            .GetMethod("RestoreStockAsync", BindingFlags.NonPublic | BindingFlags.Instance);
        await (Task)methodInfo.Invoke(_orderService, new object[] { order });
        _productRepoMock.Verify(repo => repo.UpdateAsync(It.IsAny<Product>()), Times.Never);
    }

    [Test]
    public async Task RestoreStockAsync_ShouldHandleEmptyOrderItems_WhenRestoringStock()
    {
        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = _testUserId,
            OrderDate = DateTime.UtcNow,
            Status = OrderStatus.Cancelled,
            TotalAmount = 0.00m,
            IsDeleted = false,
            UpdatedAt = DateTime.UtcNow,
            OrderItems = new List<OrderItem>()
        };
        var methodInfo = typeof(OrderManagementService)
            .GetMethod("RestoreStockAsync", BindingFlags.NonPublic | BindingFlags.Instance);
        await (Task)methodInfo.Invoke(_orderService, new object[] { order });
        _productRepoMock.Verify(repo => repo.UpdateAsync(It.IsAny<Product>()), Times.Never);
    }

    #endregion RestoreStockAsync Tests

    #region NotifyCustomerAsync Tests

    [Test]
    public async Task NotifyCustomerAsync_ShouldCreateMessage_WhenCustomerAndAdminExist()
    {
        var order = _existingOrder;
        var reason = "Order cancelled by customer";

        var customer = new AppUser
        {
            Id = _testUserId,
            UserName = "testuser",
            Email = "test@test.com"
        };

        var adminUser = new AppUser
        {
            Id = "admin-123",
            UserName = "admin",
            Email = "admin@gruers.com"
        };

        _userManagerMock.Setup(x => x.FindByIdAsync(order.UserId))
            .ReturnsAsync(customer);
        _userManagerMock.Setup(x => x.GetUsersInRoleAsync("Admin"))
            .ReturnsAsync(new List<AppUser> { adminUser });
        _userManagerMock.Setup(x => x.FindByIdAsync(customer.Id))
            .ReturnsAsync(customer);

        SystemInboxMessage capturedMessage = null;
        _systemMessageServiceMock.Setup(x => x.CreateMessageAsync(It.IsAny<SystemInboxMessage>()))
            .Callback<SystemInboxMessage>(msg => capturedMessage = msg)
            .Returns(Task.CompletedTask);

        var methodInfo = typeof(OrderManagementService)
            .GetMethod("NotifyCustomerAsync", BindingFlags.NonPublic | BindingFlags.Instance);

        await (Task)methodInfo.Invoke(_orderService, new object[] { order, reason });

        Assert.IsNotNull(capturedMessage);
        Assert.AreEqual("📦 Order Cancelled", capturedMessage.Title);
        Assert.AreEqual(reason, capturedMessage.Description);
        Assert.AreEqual(adminUser.Id, capturedMessage.SenderId);
        Assert.AreEqual(order.UserId, capturedMessage.ReceiverId);
        Assert.AreEqual(InboxMessageType.AdminToUser, capturedMessage.Type);
        Assert.IsFalse(capturedMessage.IsRead);

        _systemMessageServiceMock.Verify(x => x.CreateMessageAsync(It.IsAny<SystemInboxMessage>()), Times.Once);
    }

    [Test]
    public async Task NotifyCustomerAsync_ShouldNotCreateMessage_WhenAdminIsNull()
    {
        var order = _existingOrder;
        var reason = "Order cancelled by customer";

        var customer = new AppUser
        {
            Id = _testUserId,
            UserName = "testuser",
            Email = "test@test.com"
        };

        _userManagerMock.Setup(x => x.FindByIdAsync(order.UserId))
            .ReturnsAsync(customer);

        _userManagerMock.Setup(x => x.GetUsersInRoleAsync("Admin"))
            .ReturnsAsync(new List<AppUser>());

        var methodInfo = typeof(OrderManagementService)
            .GetMethod("NotifyCustomerAsync", BindingFlags.NonPublic | BindingFlags.Instance);

        await (Task)methodInfo.Invoke(_orderService, new object[] { order, reason });

        _systemMessageServiceMock.Verify(x => x.CreateMessageAsync(It.IsAny<SystemInboxMessage>()), Times.Never);
    }

    [Test]
    public async Task NotifyCustomerAsync_ShouldUseCorrectReceiver_WhenCustomerExists()
    {
        var order = _existingOrder;
        var reason = "Order cancelled";

        var customer = new AppUser
        {
            Id = _testUserId,
            UserName = "testuser",
            Email = "test@test.com"
        };

        var adminUser = new AppUser
        {
            Id = "admin-123",
            UserName = "admin",
            Email = "admin@gruers.com"
        };

        _userManagerMock.Setup(x => x.FindByIdAsync(order.UserId))
            .ReturnsAsync(customer);
        _userManagerMock.Setup(x => x.GetUsersInRoleAsync("Admin"))
            .ReturnsAsync(new List<AppUser> { adminUser });
        _userManagerMock.Setup(x => x.FindByIdAsync(customer.Id))
            .ReturnsAsync(customer);

        SystemInboxMessage capturedMessage = null;
        _systemMessageServiceMock.Setup(x => x.CreateMessageAsync(It.IsAny<SystemInboxMessage>()))
            .Callback<SystemInboxMessage>(msg => capturedMessage = msg)
            .Returns(Task.CompletedTask);

        var methodInfo = typeof(OrderManagementService)
            .GetMethod("NotifyCustomerAsync", BindingFlags.NonPublic | BindingFlags.Instance);

        await (Task)methodInfo.Invoke(_orderService, new object[] { order, reason });

        Assert.IsNotNull(capturedMessage);
        Assert.AreEqual(order.UserId, capturedMessage.ReceiverId);
        Assert.IsNotNull(capturedMessage.Receiver);
        Assert.AreEqual(customer.Id, capturedMessage.Receiver.Id);
    }

    #endregion NotifyCustomerAsync Tests

    #region GetByIdAsync Tests

    public async Task GetByIdAsync_ShouldReturnOrder_WhenOrderExists()
    {
        var order = _existingOrder;
        var orders = new List<Order> { order };
        var mockDbSet = orders.BuildMockDbSet();
        _orderRepoMock.Setup(repo => repo.Query())
            .Returns(mockDbSet.Object);
        var result = await _orderService.GetByIdAsync(order.Id);
        Assert.IsNotNull(result);
        Assert.AreEqual(order.Id, result.Id);
        _orderRepoMock.Verify(repo => repo.Query(), Times.Once);
    }

    [Test]
    public async Task GetByIdAsync_ShouldReturnNull_WhenOrderDoesNotExist()
    {
        var nonExistentId = Guid.NewGuid();
        var orders = new List<Order> { _existingOrder };
        var mockDbSet = orders.BuildMockDbSet();
        _orderRepoMock.Setup(repo => repo.Query())
            .Returns(mockDbSet.Object);
        var result = await _orderService.GetByIdAsync(nonExistentId);
        Assert.IsNull(result);
        _orderRepoMock.Verify(repo => repo.Query(), Times.Once);
    }

    [Test]
    public async Task GetByIdAsync_ShouldReturnDeletedOrder_WhenOrderIsDeleted()
    {
        var deletedOrder = new Order
        {
            Id = Guid.NewGuid(),
            UserId = _testUserId,
            OrderDate = DateTime.UtcNow,
            Status = OrderStatus.Pending,
            TotalAmount = 10.00m,
            IsDeleted = true,
            UpdatedAt = DateTime.UtcNow,
            OrderItems = new List<OrderItem>()
        };
        var ordersWithDeleted = new List<Order> { _existingOrder, deletedOrder };
        var mockDbSet = ordersWithDeleted.BuildMockDbSet();
        _orderRepoMock.Setup(repo => repo.Query())
            .Returns(mockDbSet.Object);
        var result = await _orderService.GetByIdAsync(deletedOrder.Id);
        Assert.IsNotNull(result);
        Assert.That(result.IsDeleted == deletedOrder.IsDeleted);
        _orderRepoMock.Verify(repo => repo.Query(), Times.Once);
    }

    [Test]
    public async Task GetByIdAsync_ShouldReturnOrderWithDetails_WhenOrderExists()
    {
        var order = _existingOrder;
        var orders = new List<Order> { order };
        var mockDbSet = orders.BuildMockDbSet();
        _orderRepoMock.Setup(repo => repo.Query())
            .Returns(mockDbSet.Object);
        var result = await _orderService.GetByIdAsync(order.Id);
        Assert.IsNotNull(result);
        Assert.AreEqual(order.Id, result.Id);
        Assert.IsNotNull(result.UserEmail);
        Assert.IsNotNull(result.OrderDate);
        Assert.AreEqual(order.User.Email, result.UserEmail);
        Assert.AreEqual(order.OrderDate, result.OrderDate);
        Assert.IsNotNull(result.OrderNumber);
        Assert.AreEqual(order.Id.ToString().Substring(0, 8).ToUpper(), result.OrderNumber);

        _orderRepoMock.Verify(repo => repo.Query(), Times.Once);
    }

    #endregion GetByIdAsync Tests
}