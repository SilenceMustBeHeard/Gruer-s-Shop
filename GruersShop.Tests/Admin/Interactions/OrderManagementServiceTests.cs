using GruersShop.Data.Repositories.Interfaces.Bakery;
using GruersShop.Data.Repositories.Interfaces.Interactions;
using GruersShop.Data.Repositories.Interfaces.Messages;
using GruersShop.Services.Core.Service.Implementations.Interactions;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GruersShop.Unit.Tests.Services.Admin.Interactions;

[TestFixture]
public class OrderManagementServiceTests
{
    private Mock<IOrderRepository> _orderRepoMock;
    private Mock<IProductRepository> _productRepoMock;
    private Mock<ISystemInboxMessageRepository> _systemMessageRepoMock;
    private OrderManagementService _orderService;

}
