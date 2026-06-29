using GruersShop.Data.Common.Enums;
using GruersShop.Data.Models.Base;
using GruersShop.Data.Models.Messages;
using GruersShop.Data.Repositories.Interfaces.Messages;
using GruersShop.Services.Core.Service.Implementations.Messages;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace GruersShop.Unit.Tests.Services.User.Messages;

[TestFixture]
public class SystemInboxClientServiceTests
{
    private Mock<ISystemInboxMessageRepository> _messageRepositoryMock;
    private SystemInboxClientService _systemInboxClientService;

    private string _testUserId;
    private string _testOtherUserId;
    private Guid _testMessageId1;
    private Guid _testMessageId2;
    private Guid _testMessageId3;
    private SystemInboxMessage _testMessageUnread;
    private SystemInboxMessage _testMessageRead;
    private SystemInboxMessage _testMessageOtherUser;
    private List<SystemInboxMessage> _testMessages;
    private List<AppUser> _testUsers;

    [SetUp]
    public void SetUp()
    {
        _messageRepositoryMock = new Mock<ISystemInboxMessageRepository>(MockBehavior.Strict);
        _systemInboxClientService = new SystemInboxClientService(_messageRepositoryMock.Object);
        SeedTestData();
    }

    private void SeedTestData()
    {
        _testUserId = "test-user-123";
        _testOtherUserId = "other-user-789";
        _testMessageId1 = Guid.NewGuid();
        _testMessageId2 = Guid.NewGuid();
        _testMessageId3 = Guid.NewGuid();

        var testUser = new AppUser
        {
            Id = _testUserId,
            UserName = "user@gruers.com",
            FirstName = "Test",
            LastName = "User",
            Email = "user@gruers.com"
        };

        var otherUser = new AppUser
        {
            Id = _testOtherUserId,
            UserName = "other@gruers.com",
            FirstName = "Other",
            LastName = "User",
            Email = "other@gruers.com"
        };

        var adminUser = new AppUser
        {
            Id = "admin-456",
            UserName = "admin@gruers.com",
            FirstName = "Admin",
            LastName = "User",
            Email = "admin@gruers.com"
        };

        _testMessageUnread = new SystemInboxMessage
        {
            Id = _testMessageId1,
            Title = "Important Announcement",
            Description = "This is an important system message",
            SenderId = adminUser.Id,
            ReceiverId = _testUserId,
            Receiver = testUser,
            Sender = adminUser,
            Type = InboxMessageType.System,

            IsRead = false,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            Priority = 1
        };

        _testMessageRead = new SystemInboxMessage
        {
            Id = _testMessageId2,
            Title = "Read Message",
            Description = "This message was already read",
            SenderId = adminUser.Id,
            ReceiverId = _testUserId,
            Receiver = testUser,
            Sender = adminUser,
            Type = InboxMessageType.AdminToUser,
            IsRead = true,
            CreatedAt = DateTime.UtcNow.AddDays(-2),
            Priority = 0
        };

        _testMessageOtherUser = new SystemInboxMessage
        {
            Id = _testMessageId3,
            Title = "Other User Message",
            Description = "This is for another user",
            SenderId = adminUser.Id,
            ReceiverId = _testOtherUserId,
            Receiver = otherUser,
            Sender = adminUser,
            Type = InboxMessageType.System,
            IsRead = false,
            CreatedAt = DateTime.UtcNow.AddDays(-3),
            Priority = 0
        };

        _testMessages = new List<SystemInboxMessage>
        {
            _testMessageUnread,
            _testMessageRead,
            _testMessageOtherUser
        };

        _testUsers = new List<AppUser>
        {
            testUser,
            otherUser,
            adminUser
        };
    }

    #region GetUserMessagesAsync Tests

    [Test]
    public async Task GetUserMessagesAsync_ReturnsOnlyMessagesForSpecifiedUser()
    {
        var mockDbSet = _testMessages.BuildMockDbSet();
        _messageRepositoryMock.Setup(x => x.GetAllAttachedAsync())
            .Returns(mockDbSet.Object);

        var result = await _systemInboxClientService.GetUserMessagesAsync(_testUserId);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(2));
    }

    [Test]
    public async Task GetUserMessagesAsync_ReturnsEmptyList_WhenUserHasNoMessages()
    {
        var emptyList = new List<SystemInboxMessage>();
        var mockDbSet = emptyList.BuildMockDbSet();
        _messageRepositoryMock.Setup(x => x.GetAllAttachedAsync())
            .Returns(mockDbSet.Object);

        var result = await _systemInboxClientService.GetUserMessagesAsync("non-existent-user");

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetUserMessagesAsync_OrdersMessagesByCreatedAtDescending()
    {
        var mockDbSet = _testMessages.BuildMockDbSet();
        _messageRepositoryMock.Setup(x => x.GetAllAttachedAsync())
            .Returns(mockDbSet.Object);

        var result = await _systemInboxClientService.GetUserMessagesAsync(_testUserId);
        var resultList = result.ToList();

        Assert.That(resultList[0].CreatedOn, Is.GreaterThan(resultList[1].CreatedOn));
    }

    [Test]
    public async Task GetUserMessagesAsync_MapsToViewModelCorrectly()
    {
        var mockDbSet = _testMessages.BuildMockDbSet();
        _messageRepositoryMock.Setup(x => x.GetAllAttachedAsync())
            .Returns(mockDbSet.Object);

        var result = await _systemInboxClientService.GetUserMessagesAsync(_testUserId);
        var firstMessage = result.First(m => m.Id == _testMessageId1);

        Assert.That(firstMessage.Id, Is.EqualTo(_testMessageUnread.Id));
        Assert.That(firstMessage.Title, Is.EqualTo(_testMessageUnread.Title));
        Assert.That(firstMessage.Description, Is.EqualTo(_testMessageUnread.Description));
        Assert.That(firstMessage.IsRead, Is.EqualTo(_testMessageUnread.IsRead));
        Assert.That(firstMessage.Type, Is.EqualTo(_testMessageUnread.Type));
    }

    #endregion GetUserMessagesAsync Tests

    #region GetMessageDetailsAsync Tests

    [Test]
    public async Task GetMessageDetailsAsync_ReturnsMessage_WhenMessageExistsAndBelongsToUser()
    {
        var messages = new List<SystemInboxMessage> { _testMessageUnread };
        var mockDbSet = messages.BuildMockDbSet();
        _messageRepositoryMock.Setup(x => x.GetAllAttachedAsync())
            .Returns(mockDbSet.Object);
        _messageRepositoryMock.Setup(x => x.UpdateAsync(_testMessageUnread))
            .ReturnsAsync(true);

        var result = await _systemInboxClientService.GetMessageDetailsAsync(_testMessageId1, _testUserId);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(_testMessageId1));
        Assert.That(result.Title, Is.EqualTo(_testMessageUnread.Title));
    }

    [Test]
    public async Task GetMessageDetailsAsync_MarksMessageAsRead_WhenMessageIsUnread()
    {
        Assert.That(_testMessageUnread.IsRead, Is.False, "Setup: Message should be unread");

        var messages = new List<SystemInboxMessage> { _testMessageUnread };
        var mockDbSet = messages.BuildMockDbSet();
        _messageRepositoryMock.Setup(x => x.GetAllAttachedAsync())
            .Returns(mockDbSet.Object);
        _messageRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<SystemInboxMessage>()))
            .ReturnsAsync(true);

        var result = await _systemInboxClientService.GetMessageDetailsAsync(_testMessageId1, _testUserId);

        _messageRepositoryMock.Verify(x => x.UpdateAsync(It.Is<SystemInboxMessage>(m => m.IsRead == true)), Times.Once);
        Assert.That(result!.IsRead, Is.True);
    }

    [Test]
    public async Task GetMessageDetailsAsync_DoesNotMarkAsRead_WhenMessageIsAlreadyRead()
    {
        Assert.That(_testMessageRead.IsRead, Is.True, "Setup: Message should be already read");

        var messages = new List<SystemInboxMessage> { _testMessageRead };
        var mockDbSet = messages.BuildMockDbSet();
        _messageRepositoryMock.Setup(x => x.GetAllAttachedAsync())
            .Returns(mockDbSet.Object);

        var result = await _systemInboxClientService.GetMessageDetailsAsync(_testMessageId2, _testUserId);

        _messageRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<SystemInboxMessage>()), Times.Never);
        Assert.That(result!.IsRead, Is.True);
    }

    [Test]
    public async Task GetMessageDetailsAsync_ReturnsNull_WhenMessageNotFound()
    {
        var emptyList = new List<SystemInboxMessage>();
        var mockDbSet = emptyList.BuildMockDbSet();
        _messageRepositoryMock.Setup(x => x.GetAllAttachedAsync())
            .Returns(mockDbSet.Object);

        var result = await _systemInboxClientService.GetMessageDetailsAsync(Guid.NewGuid(), _testUserId);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetMessageDetailsAsync_ReturnsNull_WhenMessageBelongsToDifferentUser()
    {
        var messages = new List<SystemInboxMessage> { _testMessageOtherUser };
        var mockDbSet = messages.BuildMockDbSet();
        _messageRepositoryMock.Setup(x => x.GetAllAttachedAsync())
            .Returns(mockDbSet.Object);

        var result = await _systemInboxClientService.GetMessageDetailsAsync(_testMessageId3, _testUserId);

        Assert.That(result, Is.Null);
    }

    #endregion GetMessageDetailsAsync Tests

    #region GetUnreadCountAsync Tests

    [Test]
    public async Task GetUnreadCountAsync_ReturnsCorrectUnreadCount()
    {
        var mockDbSet = _testMessages.BuildMockDbSet();
        _messageRepositoryMock.Setup(x => x.GetAllAttachedAsync())
            .Returns(mockDbSet.Object);

        var result = await _systemInboxClientService.GetUnreadCountAsync(_testUserId);

        Assert.That(result, Is.EqualTo(1));
    }

    [Test]
    public async Task GetUnreadCountAsync_ReturnsZero_WhenUserHasNoUnreadMessages()
    {
        var receiverId = _testUserId;
        var receiver = _testUsers.First(u => u.Id == receiverId);
        var allReadMessages = new List<SystemInboxMessage>
        {
            new SystemInboxMessage
            {
                Id = Guid.NewGuid(),
                ReceiverId = receiverId,
                Receiver = receiver,
                Title = "Test message 1",
                Description = "Test message 1",
                IsRead = true
            },

            new SystemInboxMessage
            {
                Id = Guid.NewGuid(),
                ReceiverId = receiverId,
                Receiver = receiver,
                Title = "Test message 2",
                Description = "Test message 2",
                IsRead = true
            }
        };
        var mockDbSet = allReadMessages.BuildMockDbSet();
        _messageRepositoryMock.Setup(x => x.GetAllAttachedAsync())
            .Returns(mockDbSet.Object);

        var result = await _systemInboxClientService.GetUnreadCountAsync(_testUserId);

        Assert.That(result, Is.EqualTo(0));
    }

    [Test]
    public async Task GetUnreadCountAsync_ReturnsZero_WhenUserHasNoMessages()
    {
        var emptyList = new List<SystemInboxMessage>();
        var mockDbSet = emptyList.BuildMockDbSet();
        _messageRepositoryMock.Setup(x => x.GetAllAttachedAsync())
            .Returns(mockDbSet.Object);

        var result = await _systemInboxClientService.GetUnreadCountAsync(_testUserId);

        Assert.That(result, Is.EqualTo(0));
    }

    [Test]
    public async Task GetUnreadCountAsync_OnlyCountsCurrentUserMessages()
    {
        var mockDbSet = _testMessages.BuildMockDbSet();
        _messageRepositoryMock.Setup(x => x.GetAllAttachedAsync())
            .Returns(mockDbSet.Object);

        var resultForUser = await _systemInboxClientService.GetUnreadCountAsync(_testUserId);
        var resultForOther = await _systemInboxClientService.GetUnreadCountAsync(_testOtherUserId);

        Assert.That(resultForUser, Is.EqualTo(1));
        Assert.That(resultForOther, Is.EqualTo(1));
    }

    #endregion GetUnreadCountAsync Tests

    #region MarkAsReadAsync Tests

    [Test]
    public async Task MarkAsReadAsync_MarksMessageAsRead_WhenMessageIsUnread()
    {
        Assert.That(_testMessageUnread.IsRead, Is.False, "Setup: Message should be unread");

        var messages = new List<SystemInboxMessage> { _testMessageUnread };
        var mockDbSet = messages.BuildMockDbSet();
        _messageRepositoryMock.Setup(x => x.GetAllAttachedAsync())
            .Returns(mockDbSet.Object);
        _messageRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<SystemInboxMessage>()))
            .ReturnsAsync(true);

        await _systemInboxClientService.MarkAsReadAsync(_testMessageId1, _testUserId);

        _messageRepositoryMock.Verify(x => x.UpdateAsync(It.Is<SystemInboxMessage>(m => m.IsRead == true)), Times.Once);
    }

    [Test]
    public async Task MarkAsReadAsync_DoesNothing_WhenMessageIsAlreadyRead()
    {
        Assert.That(_testMessageRead.IsRead, Is.True, "Setup: Message should be already read");

        var messages = new List<SystemInboxMessage> { _testMessageRead };
        var mockDbSet = messages.BuildMockDbSet();
        _messageRepositoryMock.Setup(x => x.GetAllAttachedAsync())
            .Returns(mockDbSet.Object);

        await _systemInboxClientService.MarkAsReadAsync(_testMessageId2, _testUserId);

        _messageRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<SystemInboxMessage>()), Times.Never);
    }

    [Test]
    public async Task MarkAsReadAsync_DoesNothing_WhenMessageNotFound()
    {
        var emptyList = new List<SystemInboxMessage>();
        var mockDbSet = emptyList.BuildMockDbSet();
        _messageRepositoryMock.Setup(x => x.GetAllAttachedAsync())
            .Returns(mockDbSet.Object);

        await _systemInboxClientService.MarkAsReadAsync(Guid.NewGuid(), _testUserId);

        _messageRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<SystemInboxMessage>()), Times.Never);
    }

    [Test]
    public async Task MarkAsReadAsync_DoesNothing_WhenMessageBelongsToDifferentUser()
    {
        var messages = new List<SystemInboxMessage> { _testMessageOtherUser };
        var mockDbSet = messages.BuildMockDbSet();
        _messageRepositoryMock.Setup(x => x.GetAllAttachedAsync())
            .Returns(mockDbSet.Object);

        await _systemInboxClientService.MarkAsReadAsync(_testMessageId3, _testUserId);

        _messageRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<SystemInboxMessage>()), Times.Never);
    }

    #endregion MarkAsReadAsync Tests
}