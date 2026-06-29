using GruersShop.Data.Models.Base;
using GruersShop.Services.Core.Service.Admin.Implementations.Interactions;
using GruersShop.Web.ViewModels.Admin;
using Microsoft.AspNetCore.Identity;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace GruersShop.Unit.Tests.Services.Admin.Interactions;

[TestFixture]
public class UserManagementServiceTests
{
    private Mock<UserManager<AppUser>> _userManagerMock;
    private UserManagementService _userService;

    private string _adminId;
    private string _testUserId1;
    private string _testUserId2;
    private string _testUserId3;
    private List<AppUser> _testUsers;

    [SetUp]
    public void SetUp()
    {
        var store = new Mock<IUserStore<AppUser>>();
        _userManagerMock = new Mock<UserManager<AppUser>>(
            store.Object, null, null, null, null, null, null, null, null);

        _userService = new UserManagementService(_userManagerMock.Object);

        SeedTestData();
    }

    private void SeedTestData()
    {
        _adminId = "11111111-1111-1111-1111-111111111111";
        _testUserId1 = "22222222-2222-2222-2222-222222222222";
        _testUserId2 = "33333333-3333-3333-3333-333333333333";
        _testUserId3 = "44444444-4444-4444-4444-444444444444";

        _testUsers = new List<AppUser>
            {
                new AppUser
                {
                    Id = _adminId,
                    Email = "admin@example.com",
                    UserName = "admin@example.com",
                    LockoutEnd = null
                },
                new AppUser
                {
                    Id = _testUserId1,
                    Email = "user1@example.com",
                    UserName = "user1@example.com",
                    LockoutEnd = null
                },
                new AppUser
                {
                    Id = _testUserId2,
                    Email = "user2@example.com",
                    UserName = "user2@example.com",
                    LockoutEnd = DateTimeOffset.MaxValue
                },
                new AppUser
                {
                    Id = _testUserId3,
                    Email = "user3@example.com",
                    UserName = "user3@example.com",
                    LockoutEnd = null
                }
            };
    }

    #region GetUserManagmentBoardDataAsync Tests

    [Test]
    public async Task GetUserManagmentBoardDataAsync_ReturnsAllUsersExceptAdmin()
    {
        var mockQueryable = _testUsers.BuildMockDbSet();

        _userManagerMock.Setup(u => u.Users).Returns(mockQueryable.Object);

        _userManagerMock.Setup(u => u.GetRolesAsync(It
            .Is<AppUser>(user => user.Id == _testUserId1)))
            .ReturnsAsync(new List<string> { "User" });

        _userManagerMock.Setup(u => u.GetRolesAsync(It
            .Is<AppUser>(user => user.Id == _testUserId2)))
            .ReturnsAsync(new List<string> { "User" });

        _userManagerMock.Setup(u => u.GetRolesAsync(It
            .Is<AppUser>(user => user.Id == _testUserId3)))
            .ReturnsAsync(new List<string> { "Manager" });

        var result = await _userService.GetUserManagmentBoardDataAsync(Guid.Parse(_adminId));

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count(), Is.EqualTo(3));
        Assert.That(result.Any(u => u.Id.ToString() == _adminId), Is.False);
        Assert.That(result.First().Email, Is.EqualTo("user1@example.com"));
    }

    [Test]
    public async Task GetUserManagmentBoardDataAsync_ReturnsEmptyList_WhenOnlyAdminExists()
    {
        var onlyAdmin = new List<AppUser> { _testUsers[0] };

        var mockQueryable = onlyAdmin.BuildMockDbSet();

        _userManagerMock.Setup(u => u.Users).Returns(mockQueryable.Object);

        var result = await _userService.GetUserManagmentBoardDataAsync(Guid.Parse(_adminId));

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetUserManagmentBoardDataAsync_ReturnsCorrectRoleInformation()
    {
        var mockQueryable = _testUsers.BuildMockDbSet();
        _userManagerMock.Setup(u => u.Users).Returns(mockQueryable.Object);

        _userManagerMock.Setup(u => u.GetRolesAsync(It
            .Is<AppUser>(user => user.Id == _testUserId1)))
            .ReturnsAsync(new List<string> { "User" });

        _userManagerMock.Setup(u => u.GetRolesAsync(It
            .Is<AppUser>(user => user.Id == _testUserId2)))
            .ReturnsAsync(new List<string> { "Manager" });

        _userManagerMock.Setup(u => u.GetRolesAsync(It
            .Is<AppUser>(user => user.Id == _testUserId3)))
            .ReturnsAsync(new List<string> { "Admin" });

        var result = await _userService.GetUserManagmentBoardDataAsync(Guid.Parse(_adminId));

        var user1 = result.First(u => u.Email == "user1@example.com");
        var user2 = result.First(u => u.Email == "user2@example.com");
        var user3 = result.First(u => u.Email == "user3@example.com");

        Assert.That(user1.Roles, Contains.Item("User"));
        Assert.That(user2.Roles, Contains.Item("Manager"));
        Assert.That(user3.Roles, Contains.Item("Admin"));
    }

    #endregion GetUserManagmentBoardDataAsync Tests

    #region ChangeUserRoleAsync Tests

    [Test]
    public async Task ChangeUserRoleAsync_ChangesRoleSuccessfully()
    {
        var model = new ChangeUserRoleViewModel
        {
            UserId = Guid.Parse(_testUserId1),
            NewRole = "Manager"
        };

        var user = _testUsers.First(u => u.Id == _testUserId1);

        _userManagerMock.Setup(u => u.FindByIdAsync(_testUserId1))
            .ReturnsAsync(user);

        _userManagerMock.Setup(u => u.GetRolesAsync(user))
            .ReturnsAsync(new List<string> { "User" });

        _userManagerMock.Setup(u => u.RemoveFromRolesAsync(user, It.IsAny<IEnumerable<string>>()))
            .ReturnsAsync(IdentityResult.Success);

        _userManagerMock.Setup(u => u.AddToRoleAsync(user, "Manager"))
            .ReturnsAsync(IdentityResult.Success);

        _userManagerMock.Setup(u => u.UpdateSecurityStampAsync(user))
            .ReturnsAsync(IdentityResult.Success);

        var result = await _userService.ChangeUserRoleAsync(model, Guid.Parse(_adminId));

        Assert.That(result.Failed, Is.False);
        Assert.That(result.ErrorMessage, Is.Empty);

        _userManagerMock.Verify(u => u.FindByIdAsync(_testUserId1), Times.Once);
        _userManagerMock.Verify(u => u.RemoveFromRolesAsync(user, It.IsAny<IEnumerable<string>>()), Times.Once);
        _userManagerMock.Verify(u => u.AddToRoleAsync(user, "Manager"), Times.Once);
        _userManagerMock.Verify(u => u.UpdateSecurityStampAsync(user), Times.Once);
    }

    [Test]
    public async Task ChangeUserRoleAsync_ReturnsError_WhenUserNotFound()
    {
        var model = new ChangeUserRoleViewModel
        {
            UserId = Guid.NewGuid(),
            NewRole = "Manager"
        };

        _userManagerMock.Setup(u => u.FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((AppUser)null!);

        var result = await _userService.ChangeUserRoleAsync(model, Guid.Parse(_adminId));

        Assert.That(result.Failed, Is.True);
        Assert.That(result.ErrorMessage, Is.EqualTo("User not found."));

        _userManagerMock.Verify(u => u.FindByIdAsync(It.IsAny<string>()), Times.Once);
        _userManagerMock.Verify(u => u.RemoveFromRolesAsync(It.IsAny<AppUser>(), It.IsAny<IEnumerable<string>>()), Times.Never);
        _userManagerMock.Verify(u => u.AddToRoleAsync(It.IsAny<AppUser>(), It.IsAny<string>()), Times.Never);
        _userManagerMock.Verify(u => u.UpdateSecurityStampAsync(It.IsAny<AppUser>()), Times.Never);
    }

    [Test]
    public async Task ChangeUserRoleAsync_ReturnsError_WhenRemoveRolesFails()
    {
        var model = new ChangeUserRoleViewModel
        {
            UserId = Guid.Parse(_testUserId1),
            NewRole = "Manager"
        };

        var user = _testUsers.First(u => u.Id == _testUserId1);

        _userManagerMock.Setup(u => u.FindByIdAsync(_testUserId1))
            .ReturnsAsync(user);

        _userManagerMock.Setup(u => u.GetRolesAsync(user))
            .ReturnsAsync(new List<string> { "User" });

        _userManagerMock.Setup(u => u.RemoveFromRolesAsync(user, It.IsAny<IEnumerable<string>>()))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Remove failed" }));

        var result = await _userService.ChangeUserRoleAsync(model, Guid.Parse(_adminId));

        Assert.That(result.Failed, Is.True);
        Assert.That(result.ErrorMessage, Is.EqualTo("Failed to remove existing roles."));
        _userManagerMock.Verify(u => u.FindByIdAsync(_testUserId1), Times.Once);
        _userManagerMock.Verify(u => u.RemoveFromRolesAsync(user, It.IsAny<IEnumerable<string>>()), Times.Once);
        _userManagerMock.Verify(u => u.AddToRoleAsync(It.IsAny<AppUser>(), It.IsAny<string>()), Times.Never);
        _userManagerMock.Verify(u => u.UpdateSecurityStampAsync(It.IsAny<AppUser>()), Times.Never);
    }

    [Test]
    public async Task ChangeUserRoleAsync_ReturnsError_WhenAddRoleFails()
    {
        var model = new ChangeUserRoleViewModel
        {
            UserId = Guid.Parse(_testUserId1),
            NewRole = "Manager"
        };

        var user = _testUsers.First(u => u.Id == _testUserId1);

        _userManagerMock.Setup(u => u.FindByIdAsync(_testUserId1))
            .ReturnsAsync(user);

        _userManagerMock.Setup(u => u.GetRolesAsync(user))
            .ReturnsAsync(new List<string> { "User" });

        _userManagerMock.Setup(u => u.RemoveFromRolesAsync(user, It.IsAny<IEnumerable<string>>()))
            .ReturnsAsync(IdentityResult.Success);

        _userManagerMock.Setup(u => u.AddToRoleAsync(user, "Manager"))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Add failed" }));

        var result = await _userService.ChangeUserRoleAsync(model, Guid.Parse(_adminId));

        Assert.That(result.Failed, Is.True);
        Assert.That(result.ErrorMessage, Is.EqualTo("Failed to assign new role."));

        _userManagerMock.Verify(u => u.FindByIdAsync(_testUserId1), Times.Once);
        _userManagerMock.Verify(u => u.RemoveFromRolesAsync(user, It.IsAny<IEnumerable<string>>()), Times.Once);
        _userManagerMock.Verify(u => u.AddToRoleAsync(user, "Manager"), Times.Once);
        _userManagerMock.Verify(u => u.UpdateSecurityStampAsync(It.IsAny<AppUser>()), Times.Never);
    }

    [Test]
    public async Task ChangeUserRoleAsync_UpdatesSecurityStamp_EvenWhenRolesDontChange()
    {
        var model = new ChangeUserRoleViewModel
        {
            UserId = Guid.Parse(_testUserId1),
            NewRole = "User"
        };

        var user = _testUsers.First(u => u.Id == _testUserId1);

        _userManagerMock.Setup(u => u.FindByIdAsync(_testUserId1))
            .ReturnsAsync(user);

        _userManagerMock.Setup(u => u.GetRolesAsync(user))
            .ReturnsAsync(new List<string> { "User" });

        _userManagerMock.Setup(u => u.RemoveFromRolesAsync(user, It.IsAny<IEnumerable<string>>()))
            .ReturnsAsync(IdentityResult.Success);

        _userManagerMock.Setup(u => u.AddToRoleAsync(user, "User"))
            .ReturnsAsync(IdentityResult.Success);

        _userManagerMock.Setup(u => u.UpdateSecurityStampAsync(user))
            .ReturnsAsync(IdentityResult.Success);

        var result = await _userService.ChangeUserRoleAsync(model, Guid.Parse(_adminId));

        Assert.That(result.Failed, Is.False);
        _userManagerMock.Verify(u => u.UpdateSecurityStampAsync(user), Times.Once);
    }

    [Test]
    public async Task ChangeUserRoleAsync_ReturnsError_WhenUpdateSecurityStampFails()
    {
        var model = new ChangeUserRoleViewModel
        {
            UserId = Guid.Parse(_testUserId1),
            NewRole = "Manager"
        };

        var user = _testUsers.First(u => u.Id == _testUserId1);

        _userManagerMock.Setup(u => u.FindByIdAsync(_testUserId1))
            .ReturnsAsync(user);

        _userManagerMock.Setup(u => u.GetRolesAsync(user))
            .ReturnsAsync(new List<string> { "User" });

        _userManagerMock.Setup(u => u.RemoveFromRolesAsync(user, It.IsAny<IEnumerable<string>>()))
            .ReturnsAsync(IdentityResult.Success);

        _userManagerMock.Setup(u => u.AddToRoleAsync(user, "Manager"))
            .ReturnsAsync(IdentityResult.Success);

        _userManagerMock.Setup(u => u.UpdateSecurityStampAsync(user))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Security stamp update failed" }));

        var result = await _userService.ChangeUserRoleAsync(model, Guid.Parse(_adminId));

        Assert.That(result.Failed, Is.True);
        Assert.That(result.ErrorMessage, Is.EqualTo("Failed to update security stamp."));
        _userManagerMock.Verify(u => u.UpdateSecurityStampAsync(user), Times.Once);
    }

    #endregion ChangeUserRoleAsync Tests

    #region FindUserByIdAsync Tests

    [Test]
    public async Task FindUserByIdAsync_ReturnsUser_WhenUserExists()
    {
        var mockQueryable = _testUsers.BuildMockDbSet();

        _userManagerMock.Setup(u => u.Users).Returns(mockQueryable.Object);

        _userManagerMock.Setup(u => u.GetRolesAsync(It.Is<AppUser>(user => user.Id == _testUserId1)))
            .ReturnsAsync(new List<string> { "User" });

        var result = await _userService.FindUserByIdAsync(_testUserId1);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id.ToString(), Is.EqualTo(_testUserId1));
        Assert.That(result.Email, Is.EqualTo("user1@example.com"));
        Assert.That(result.Roles, Contains.Item("User"));
    }

    [Test]
    public async Task FindUserByIdAsync_ReturnsNull_WhenUserNotFound()
    {
        var emptyList = new List<AppUser>();
        var mockQueryable = emptyList.BuildMockDbSet();

        _userManagerMock.Setup(u => u.Users).Returns(mockQueryable.Object);

        var result = await _userService.FindUserByIdAsync("99999999-9999-9999-9999-999999999999");

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task FindUserByIdAsync_ReturnsUserWithLockoutInfo()
    {
        var mockQueryable = _testUsers.BuildMockDbSet();
        _userManagerMock.Setup(u => u.Users).Returns(mockQueryable.Object);
        _userManagerMock.Setup(u => u.GetRolesAsync(It.Is<AppUser>(user => user.Id == _testUserId2)))
            .ReturnsAsync(new List<string> { "User" });

        var result = await _userService.FindUserByIdAsync(_testUserId2);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.LockoutEnd, Is.EqualTo(DateTimeOffset.MaxValue));
    }

    #endregion FindUserByIdAsync Tests

    #region DisableUser Tests

    [Test]
    public async Task DisableUser_SetsLockoutEndToMaxValue_WhenUserExists()
    {
        var user = _testUsers.First(u => u.Id == _testUserId1);
        var mockQueryable = _testUsers.BuildMockDbSet();
        _userManagerMock.Setup(u => u.Users).Returns(mockQueryable.Object);
        _userManagerMock.Setup(u => u.UpdateAsync(user))
            .ReturnsAsync(IdentityResult.Success);

        var result = await _userService.DisableUser(_testUserId1);

        Assert.That(result.Failed, Is.False);
        Assert.That(result.ErrorMessage, Is.Empty);
        Assert.That(user.LockoutEnd, Is.EqualTo(DateTimeOffset.MaxValue));
        _userManagerMock.Verify(u => u.UpdateAsync(user), Times.Once);
    }

    [Test]
    public async Task DisableUser_ReturnsError_WhenUserNotFound()
    {
        var emptyList = new List<AppUser>();
        var mockQueryable = emptyList.BuildMockDbSet();
        _userManagerMock.Setup(u => u.Users).Returns(mockQueryable.Object);

        var result = await _userService.DisableUser("99999999-9999-9999-9999-999999999999");

        Assert.That(result.Failed, Is.True);
        Assert.That(result.ErrorMessage, Is.EqualTo("User not found."));
        _userManagerMock.Verify(u => u.UpdateAsync(It.IsAny<AppUser>()), Times.Never);
    }

    [Test]
    public async Task DisableUser_OverridesExistingLockoutEnd()
    {
        var user = _testUsers.First(u => u.Id == _testUserId2);

        var mockQueryable = _testUsers.BuildMockDbSet();

        _userManagerMock.Setup(u => u.Users).Returns(mockQueryable.Object);
        _userManagerMock.Setup(u => u.UpdateAsync(user))
            .ReturnsAsync(IdentityResult.Success);

        var result = await _userService.DisableUser(_testUserId2);

        Assert.That(result.Failed, Is.False);
        Assert.That(user.LockoutEnd, Is.EqualTo(DateTimeOffset.MaxValue));
    }

    #endregion DisableUser Tests
}