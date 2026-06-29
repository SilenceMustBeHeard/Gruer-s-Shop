using GruersShop.Data.Common.Enums;
using GruersShop.Data.Models.Base;
using GruersShop.Data.Models.Messages;
using GruersShop.Data.Repositories.Interfaces.Messages;
using GruersShop.Services.Core.Service.Implementations.Messages;
using GruersShop.Web.ViewModels.Account.Messages;
using Microsoft.AspNetCore.Identity;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;
using System.Security.Claims;
using Assert = NUnit.Framework.Assert;

namespace GruersShop.Unit.Tests.Services.Messages;

[TestFixture]
public class ContactMessageClientServiceTests
{
    private Mock<IContactMessageRepository> _messageRepositoryMock;
    private Mock<UserManager<AppUser>> _userManagerMock;
    private ContactMessageClientService _contactMessageClientService;

    private string _testUserId;
    private string _testAdminId;
    private string _testOtherUserId;
    private Guid _testMessageId;
    private Guid _testMessageId2;
    private ContactMessage _testMessage;
    private ContactMessage _testMessageWithResponse;
    private ContactMessage _testMessageRead;
    private ContactMessage _testMessageOtherUser;
    private List<ContactMessage> _testMessages;
    private List<AppUser> _testUsers;

    [SetUp]
    public void SetUp()
    {
        _messageRepositoryMock = new Mock<IContactMessageRepository>(MockBehavior.Strict);

        var store = new Mock<IUserStore<AppUser>>();
        _userManagerMock = new Mock<UserManager<AppUser>>(
            store.Object, null, null, null, null, null, null, null, null);

        _contactMessageClientService = new ContactMessageClientService(
            _messageRepositoryMock.Object,
            _userManagerMock.Object);

        SeedTestData();
    }

    private void SeedTestData()
    {
        _testUserId = "test-user-123";
        _testAdminId = "admin-456";
        _testOtherUserId = "other-user-789";
        _testMessageId = Guid.NewGuid();
        _testMessageId2 = Guid.NewGuid();

        var testAdmin = new AppUser
        {
            Id = _testAdminId,
            UserName = "admin@gruers.com",
            FirstName = "Admin",
            LastName = "User",
            Email = "admin@gruers.com"
        };

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

        _testMessage = new ContactMessage
        {
            Id = _testMessageId,
            SenderId = _testUserId,
            ReceiverId = _testAdminId,
            Subject = "Test Subject",
            Message = "Test Message with sufficient length",
            Type = InboxMessageType.UserToAdmin,
            CreatedAt = DateTime.UtcNow.AddDays(-2),
            IsRead = false,
            IsReadByAdmin = false,
            Response = null,
            RespondedAt = null,
            Sender = testUser,
            Receiver = testAdmin
        };

        _testMessageWithResponse = new ContactMessage
        {
            Id = _testMessageId2,
            SenderId = _testUserId,
            ReceiverId = _testAdminId,
            Subject = "Response Subject",
            Message = "Original Message",
            Type = InboxMessageType.UserToAdmin,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            IsRead = false,
            IsReadByAdmin = true,
            Response = "This is the admin response",
            RespondedAt = DateTime.UtcNow.AddDays(-1),
            RespondedById = _testAdminId,
            Sender = testUser,
            Receiver = testAdmin,
            RespondedBy = testAdmin
        };

        _testMessageRead = new ContactMessage
        {
            Id = Guid.NewGuid(),
            SenderId = _testUserId,
            ReceiverId = _testAdminId,
            Subject = "Read Subject",
            Message = "Read Message",
            Type = InboxMessageType.UserToAdmin,
            CreatedAt = DateTime.UtcNow.AddHours(-1),
            IsRead = true,
            IsReadByAdmin = true,
            Response = "Response already read",
            RespondedAt = DateTime.UtcNow.AddHours(-1),
            Sender = testUser,
            Receiver = testAdmin,
            RespondedBy = testAdmin
        };

        _testMessageOtherUser = new ContactMessage
        {
            Id = Guid.NewGuid(),
            SenderId = _testOtherUserId,
            ReceiverId = _testAdminId,
            Subject = "Other User Subject",
            Message = "Other User Message",
            Type = InboxMessageType.UserToAdmin,
            CreatedAt = DateTime.UtcNow.AddDays(-3),
            IsRead = false,
            IsReadByAdmin = false,
            Response = null,
            Sender = otherUser,
            Receiver = testAdmin
        };

        _testMessages = new List<ContactMessage>
        {
            _testMessage,
            _testMessageWithResponse,
            _testMessageRead,
            _testMessageOtherUser
        };

        _testUsers = new List<AppUser>
        {
            testAdmin,
            testUser,
            otherUser
        };
    }

    private ClaimsPrincipal GetTestUserPrincipal(string userId)
    {
        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId) };
        var identity = new ClaimsIdentity(claims);
        return new ClaimsPrincipal(identity);
    }

    private void SetupUserManagerForSendMessage(string userId, bool hasAdmin = true)
    {
        _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(_testUsers.FirstOrDefault(u => u.Id == userId));

        var adminUsers = hasAdmin
            ? _testUsers.Where(u => u.Id == _testAdminId).ToList()
            : new List<AppUser>();

        _userManagerMock.Setup(x => x.GetUsersInRoleAsync("Admin"))
            .ReturnsAsync(adminUsers);
    }

    #region SendContactMessageAsync Tests

    [Test]
    public async Task SendContactMessageAsync_ValidMessage_CreatesContactMessage()
    {
        var model = new ContactMessageCreateViewModel
        {
            Subject = "New Test Subject",
            Message = "New Test Message with sufficient length"
        };
        var userPrincipal = GetTestUserPrincipal(_testUserId);

        SetupUserManagerForSendMessage(_testUserId);

        var emptyList = new List<ContactMessage>();
        var mockDbSet = emptyList.BuildMockDbSet();
        _messageRepositoryMock.Setup(x => x.Query())
            .Returns(mockDbSet.Object);

        _messageRepositoryMock.Setup(x => x.AddAsync(It.IsAny<ContactMessage>()))
            .Returns(Task.CompletedTask);

        _messageRepositoryMock.Setup(x => x.SaveChangesAsync())
            .Returns(Task.FromResult(1));

        await _contactMessageClientService.SendContactMessageAsync(model, userPrincipal);

        _messageRepositoryMock.Verify(x => x.AddAsync(It.Is<ContactMessage>(cm =>
            cm.SenderId == _testUserId &&
            cm.ReceiverId == _testAdminId &&
            cm.Subject == model.Subject &&
            cm.Message == model.Message &&
            cm.Type == InboxMessageType.UserToAdmin &&
            cm.IsRead == false &&
            cm.IsReadByAdmin == false)), Times.Once);

        _messageRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task SendContactMessageAsync_DuplicateMessage_DoesNotCreateDuplicate()
    {
        var model = new ContactMessageCreateViewModel
        {
            Subject = _testMessage.Subject,
            Message = _testMessage.Message
        };
        var userPrincipal = GetTestUserPrincipal(_testUserId);

        SetupUserManagerForSendMessage(_testUserId);

        var mockDbSet = _testMessages.BuildMockDbSet();
        _messageRepositoryMock.Setup(x => x.Query())
            .Returns(mockDbSet.Object);

        await _contactMessageClientService.SendContactMessageAsync(model, userPrincipal);

        _messageRepositoryMock.Verify(x => x.AddAsync(It.IsAny<ContactMessage>()), Times.Never);
        _messageRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    [Test]
    public void SendContactMessageAsync_UserNotLoggedIn_ThrowsArgumentException()
    {
        var model = new ContactMessageCreateViewModel
        {
            Subject = "Test Subject",
            Message = "Test Message with sufficient length"
        };
        var userPrincipal = GetTestUserPrincipal(_testUserId);

        _userManagerMock.Setup(x => x.GetUserAsync(userPrincipal))
            .ReturnsAsync((AppUser)null);

        var ex = Assert.ThrowsAsync<ArgumentException>(
            () => _contactMessageClientService.SendContactMessageAsync(model, userPrincipal));
        Assert.That(ex.Message, Is.EqualTo("You must be logged in to send a contact message."));
    }

    [Test]
    public void SendContactMessageAsync_NoAdminFound_ThrowsInvalidOperationException()
    {
        var model = new ContactMessageCreateViewModel
        {
            Subject = "Test Subject",
            Message = "Test Message with sufficient length"
        };
        var userPrincipal = GetTestUserPrincipal(_testUserId);

        SetupUserManagerForSendMessage(_testUserId, hasAdmin: false);

        var ex = Assert.ThrowsAsync<InvalidOperationException>(
            () => _contactMessageClientService.SendContactMessageAsync(model, userPrincipal));
        Assert.That(ex.Message, Is.EqualTo("No admin user found in the system."));
    }

    #endregion SendContactMessageAsync Tests

    #region GetUserMessagesAsync Tests

    [Test]
    public async Task GetUserMessagesAsync_ReturnsEmptyList_WhenUserHasNoMessages()
    {
        var emptyList = new List<ContactMessage>();
        var mockDbSet = emptyList.BuildMockDbSet();
        _messageRepositoryMock.Setup(x => x.Query())
            .Returns(mockDbSet.Object);

        var result = await _contactMessageClientService.GetUserMessagesAsync(_testUserId);

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }

    #endregion GetUserMessagesAsync Tests

    #region GetMessageDetailsAsync Tests

    [Test]
    public async Task GetMessageDetailsAsync_MessageNotFound_ReturnsNull()
    {
        var emptyList = new List<ContactMessage>();
        var mockDbSet = emptyList.BuildMockDbSet();
        _messageRepositoryMock.Setup(x => x.Query())
            .Returns(mockDbSet.Object);

        var result = await _contactMessageClientService.GetMessageDetailsAsync(Guid.NewGuid(), _testUserId);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetMessageDetailsAsync_UserNotSender_ReturnsNull()
    {
        var mockDbSet = _testMessages.BuildMockDbSet();
        _messageRepositoryMock.Setup(x => x.Query())
            .Returns(mockDbSet.Object);

        var result = await _contactMessageClientService.GetMessageDetailsAsync(_testMessageWithResponse.Id, _testOtherUserId);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetMessageDetailsAsync_MarksAsRead_WhenHasResponseAndNotRead()
    {
        var testAdmin = _testUsers.First(u => u.Id == _testAdminId);
        var testUser = _testUsers.First(u => u.Id == _testUserId);

        var message = new ContactMessage
        {
            Id = Guid.NewGuid(),
            SenderId = _testUserId,
            ReceiverId = _testAdminId,
            Response = "Test Response",
            IsRead = false,
            Subject = "Test",
            Message = "Content",
            CreatedAt = DateTime.UtcNow,
            Sender = testUser,
            Receiver = testAdmin,
            RespondedBy = testAdmin
        };

        var messages = new List<ContactMessage> { message };
        var mockDbSet = messages.BuildMockDbSet();
        _messageRepositoryMock.Setup(x => x.Query())
            .Returns(mockDbSet.Object);
        _messageRepositoryMock.Setup(x => x.UpdateAsync(message))
            .ReturnsAsync(true);

        var result = await _contactMessageClientService.GetMessageDetailsAsync(message.Id, _testUserId);

        Assert.That(result, Is.Not.Null);
        Assert.That(message.IsRead, Is.True);
        _messageRepositoryMock.Verify(x => x.UpdateAsync(message), Times.Once);
    }

    #endregion GetMessageDetailsAsync Tests

    #region GetUserUnreadResponsesCountAsync Tests

    [Test]
    public async Task GetUserUnreadResponsesCountAsync_ReturnsCorrectCount()
    {
        var mockDbSet = _testMessages.BuildMockDbSet();
        _messageRepositoryMock.Setup(x => x.Query())
            .Returns(mockDbSet.Object);

        var result = await _contactMessageClientService.GetUserUnreadResponsesCountAsync(_testUserId);

        Assert.That(result, Is.EqualTo(1));
    }

    [Test]
    public async Task GetUserUnreadResponsesCountAsync_ReturnsZero_WhenNoUnreadResponses()
    {
        var receiver = _testUsers.First(u => u.Id == _testAdminId);
        var messages = new List<ContactMessage>
        {
            _testMessageRead,
            new ContactMessage
            {
                Id = Guid.NewGuid(),
                SenderId = _testUserId,
                ReceiverId = _testAdminId,
                Receiver = receiver,
                Response = null,
                IsRead = false,
                Subject = "Test",
                Message = "Content"
            }
        };
        var mockDbSet = messages.BuildMockDbSet();
        _messageRepositoryMock.Setup(x => x.Query())
            .Returns(mockDbSet.Object);

        var result = await _contactMessageClientService.GetUserUnreadResponsesCountAsync(_testUserId);

        Assert.That(result, Is.EqualTo(0));
    }

    [Test]
    public async Task GetUserUnreadResponsesCountAsync_ReturnsZero_WhenUserHasNoMessages()
    {
        var emptyList = new List<ContactMessage>();
        var mockDbSet = emptyList.BuildMockDbSet();
        _messageRepositoryMock.Setup(x => x.Query())
            .Returns(mockDbSet.Object);

        var result = await _contactMessageClientService.GetUserUnreadResponsesCountAsync(_testUserId);

        Assert.That(result, Is.EqualTo(0));
    }

    #endregion GetUserUnreadResponsesCountAsync Tests

    #region MarkAsReadAsync Tests

    [Test]
    public async Task MarkAsReadAsync_ReturnsTrue_WhenMessageMarkedAsRead()
    {
        var testAdmin = _testUsers.First(u => u.Id == _testAdminId);
        var testUser = _testUsers.First(u => u.Id == _testUserId);

        var message = new ContactMessage
        {
            Id = Guid.NewGuid(),
            SenderId = _testUserId,
            ReceiverId = _testAdminId,
            Response = "Test Response",
            IsRead = false,
            Subject = "Test",
            Message = "Content",
            Sender = testUser,
            Receiver = testAdmin
        };

        var messages = new List<ContactMessage> { message };
        var mockDbSet = messages.BuildMockDbSet();
        _messageRepositoryMock.Setup(x => x.Query())
            .Returns(mockDbSet.Object);
        _messageRepositoryMock.Setup(x => x.UpdateAsync(message))
            .ReturnsAsync(true);
        _messageRepositoryMock.Setup(x => x.SaveChangesAsync())
            .Returns(Task.FromResult(1));

        var result = await _contactMessageClientService.MarkAsReadAsync(message.Id, _testUserId);

        Assert.That(result, Is.True);
        _messageRepositoryMock.Verify(x => x.UpdateAsync(message), Times.Once);
        _messageRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task MarkAsReadAsync_ReturnsFalse_WhenMessageHasNoResponse()
    {
        var testUser = _testUsers.First(u => u.Id == _testUserId);
        var receiver = _testUsers.First(u => u.Id == _testAdminId);
        var message = new ContactMessage
        {
            Id = Guid.NewGuid(),
            SenderId = _testUserId,
            ReceiverId = _testAdminId,
            Receiver = receiver,
            Response = null,
            IsRead = false,
            Subject = "Test",
            Message = "Content",
            Sender = testUser
        };

        var messages = new List<ContactMessage> { message };
        var mockDbSet = messages.BuildMockDbSet();
        _messageRepositoryMock.Setup(x => x.Query())
            .Returns(mockDbSet.Object);

        var result = await _contactMessageClientService.MarkAsReadAsync(message.Id, _testUserId);

        Assert.That(result, Is.False);
        _messageRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<ContactMessage>()), Times.Never);
    }

    [Test]
    public async Task MarkAsReadAsync_ReturnsNull_WhenMessageNotFound()
    {
        var emptyList = new List<ContactMessage>();
        var mockDbSet = emptyList.BuildMockDbSet();
        _messageRepositoryMock.Setup(x => x.Query())
            .Returns(mockDbSet.Object);

        var result = await _contactMessageClientService.MarkAsReadAsync(Guid.NewGuid(), _testUserId);

        Assert.That(result, Is.Null);
    }

    #endregion MarkAsReadAsync Tests
}