using GruersShop.Data.Models.Base;
using GruersShop.Services.Core.Service.Implementations.Account;
using GruersShop.Services.Core.Service.Interfaces.Account;
using GruersShop.Web.ViewModels.Account.Profile;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace GruersShop.Unit.Tests.Services.Account;

[TestFixture]
public class AccountServiceTests
{
    private Mock<UserManager<AppUser>> _userManagerMock;
    private Mock<SignInManager<AppUser>> _signInManagerMock;
    private Mock<IEmailService> _emailServiceMock;
    private Mock<IViewRenderService> _viewRenderServiceMock;
    private Mock<ILogger<AccountService>> _loggerMock;
    private AccountService _accountService;

    private string _testEmail;
    private string _testPassword;
    private string _testFirstName;
    private string _testLastName;
    private AppUser _testUser;
    private List<AppUser> _testUsers;

    [SetUp]
    public void SetUp()
    {
        var userStoreMock = new Mock<IUserStore<AppUser>>();
        _userManagerMock = new Mock<UserManager<AppUser>>(
            userStoreMock.Object, null, null, null, null, null, null, null, null);

        var contextAccessorMock = new Mock<IHttpContextAccessor>();
        var claimsFactoryMock = new Mock<IUserClaimsPrincipalFactory<AppUser>>();
        _signInManagerMock = new Mock<SignInManager<AppUser>>(
            _userManagerMock.Object,
            contextAccessorMock.Object,
            claimsFactoryMock.Object,
            null, null, null, null);

        _emailServiceMock = new Mock<IEmailService>();
        _viewRenderServiceMock = new Mock<IViewRenderService>();
        _loggerMock = new Mock<ILogger<AccountService>>();

        _accountService = new AccountService(
            _userManagerMock.Object,
            _signInManagerMock.Object,
            _emailServiceMock.Object,
            _viewRenderServiceMock.Object,
            _loggerMock.Object);

        SeedTestData();
    }

    private void SeedTestData()
    {
        _testEmail = "test@gruers.com";
        _testPassword = "Test123!";
        _testFirstName = "Test";
        _testLastName = "User";

        _testUser = new AppUser
        {
            Id = "test-user-123",
            UserName = _testEmail,
            Email = _testEmail,
            FirstName = _testFirstName,
            LastName = _testLastName,
            Address = "123 Test Street",
            AlternateEmail = "alt@gruers.com"
        };

        _testUsers = new List<AppUser> { _testUser };
    }

    #region RegisterAsync Tests

    [Test]
    public async Task RegisterAsync_ValidData_ReturnsSuccess()
    {
        var model = new RegisterViewModel
        {
            Email = "newuser@gruers.com",
            Password = "NewPass123!",
            ConfirmPassword = "NewPass123!",
            FirstName = "New",
            LastName = "User",
            Address = "456 New Street",
            AlternateEmail = "newalt@gruers.com"
        };

        _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<AppUser>(), model.Password))
            .ReturnsAsync(IdentityResult.Success);

        _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<AppUser>(), "User"))
            .ReturnsAsync(IdentityResult.Success);

        _signInManagerMock.Setup(x => x.SignInAsync(It.IsAny<AppUser>(), false, null))
            .Returns(Task.CompletedTask);

        var (success, errors) = await _accountService.RegisterAsync(model);

        Assert.That(success, Is.True);
        Assert.That(errors, Is.Empty);

        _userManagerMock.Verify(x => x.CreateAsync(It.Is<AppUser>(u =>
            u.Email == model.Email &&
            u.FirstName == model.FirstName), model.Password), Times.Once);
        _userManagerMock.Verify(x => x.AddToRoleAsync(It.IsAny<AppUser>(), "User"), Times.Once);
        _signInManagerMock.Verify(x => x.SignInAsync(It.IsAny<AppUser>(), false, null), Times.Once);
    }

    [Test]
    public async Task RegisterAsync_InvalidData_ReturnsFailure()
    {
        var model = new RegisterViewModel
        {
            Email = "newuser@gruers.com",
            Password = "NewPass123!",
            ConfirmPassword = "NewPass123!",
            FirstName = "New",
            LastName = "User"
        };

        var identityErrors = new[]
        {
            new IdentityError { Description = "Password too weak" },
            new IdentityError { Description = "Email already taken" }
        };

        _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<AppUser>(), model.Password))
            .ReturnsAsync(IdentityResult.Failed(identityErrors));

        var (success, errors) = await _accountService.RegisterAsync(model);

        Assert.That(success, Is.False);
        Assert.That(errors.Length, Is.EqualTo(2));
        Assert.That(errors[0], Is.EqualTo("Password too weak"));

        _userManagerMock.Verify(x => x.AddToRoleAsync(It.IsAny<AppUser>(), "User"), Times.Never);
    }

    #endregion RegisterAsync Tests

    #region LoginAsync Tests

    [Test]
    public async Task LoginAsync_ValidCredentials_ReturnsTrue()
    {
        var model = new LoginViewModel
        {
            Email = _testEmail,
            Password = _testPassword,
            RememberMe = true
        };

        _signInManagerMock.Setup(x => x.PasswordSignInAsync(
                model.Email, model.Password, model.RememberMe, false))
            .ReturnsAsync(SignInResult.Success);

        var result = await _accountService.LoginAsync(model);

        Assert.That(result, Is.True);
    }

    [Test]
    public async Task LoginAsync_InvalidCredentials_ReturnsFalse()
    {
        var model = new LoginViewModel
        {
            Email = _testEmail,
            Password = "WrongPassword",
            RememberMe = false
        };

        _signInManagerMock.Setup(x => x.PasswordSignInAsync(
                model.Email, model.Password, model.RememberMe, false))
            .ReturnsAsync(SignInResult.Failed);

        var result = await _accountService.LoginAsync(model);

        Assert.That(result, Is.False);
    }

    #endregion LoginAsync Tests

    #region LogoutAsync Tests

    [Test]
    public async Task LogoutAsync_CallsSignOutManager()
    {
        _signInManagerMock.Setup(x => x.SignOutAsync())
            .Returns(Task.CompletedTask);

        await _accountService.LogoutAsync();

        _signInManagerMock.Verify(x => x.SignOutAsync(), Times.Once);
    }

    #endregion LogoutAsync Tests

    #region ForgotPasswordAsync Tests

    [Test]
    public async Task ForgotPasswordAsync_ExistingUser_SendsEmailAndReturnsTrue()
    {
        var resetLink = "https://localhost/reset";

        _userManagerMock.Setup(x => x.FindByEmailAsync(_testEmail))
            .ReturnsAsync(_testUser);

        _userManagerMock.Setup(x => x.GeneratePasswordResetTokenAsync(_testUser))
            .ReturnsAsync("reset-token-123");

        _viewRenderServiceMock.Setup(x => x.RenderToStringAsync(
                "Emails/PasswordReset", _testUser, It.IsAny<ViewDataDictionary>()))
            .ReturnsAsync("<html>Reset link: {{ResetLink}}</html>");

        _emailServiceMock.Setup(x => x.SendEmailAsync(
                _testEmail, "Password Reset Request", It.IsAny<string>()))
            .ReturnsAsync(true);

        var result = await _accountService.ForgotPasswordAsync(_testEmail, resetLink);

        Assert.That(result, Is.True);

        _userManagerMock.Verify(x => x.FindByEmailAsync(_testEmail), Times.Once);
        _userManagerMock.Verify(x => x.GeneratePasswordResetTokenAsync(_testUser), Times.Once);
        _emailServiceMock.Verify(x => x.SendEmailAsync(_testEmail, "Password Reset Request", It.IsAny<string>()), Times.Once);
    }

    [Test]
    public async Task ForgotPasswordAsync_NonExistingUser_ReturnsFalse()
    {
        var nonExistingEmail = "nonexisting@gruers.com";
        var resetLink = "https://localhost/reset";

        _userManagerMock.Setup(x => x.FindByEmailAsync(nonExistingEmail))
            .ReturnsAsync((AppUser)null);

        var result = await _accountService.ForgotPasswordAsync(nonExistingEmail, resetLink);

        Assert.That(result, Is.False);
        _emailServiceMock.Verify(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    #endregion ForgotPasswordAsync Tests

    #region ResetPasswordAsync Tests

    [Test]
    public async Task ResetPasswordAsync_ValidToken_ReturnsSuccess()
    {
        var model = new ResetPasswordViewModel
        {
            Email = _testEmail,
            Token = "valid-token",
            Password = "NewPass123!",
            ConfirmPassword = "NewPass123!"
        };

        _userManagerMock.Setup(x => x.FindByEmailAsync(_testEmail))
            .ReturnsAsync(_testUser);

        _userManagerMock.Setup(x => x.ResetPasswordAsync(_testUser, model.Token, model.Password))
            .ReturnsAsync(IdentityResult.Success);

        var (success, errors) = await _accountService.ResetPasswordAsync(model);

        Assert.That(success, Is.True);
        Assert.That(errors, Is.Empty);
    }

    [Test]
    public async Task ResetPasswordAsync_InvalidToken_ReturnsFailure()
    {
        var model = new ResetPasswordViewModel
        {
            Email = _testEmail,
            Token = "invalid-token",
            Password = "NewPass123!",
            ConfirmPassword = "NewPass123!"
        };

        var identityErrors = new[] { new IdentityError { Description = "Invalid token" } };

        _userManagerMock.Setup(x => x.FindByEmailAsync(_testEmail))
            .ReturnsAsync(_testUser);

        _userManagerMock.Setup(x => x.ResetPasswordAsync(_testUser, model.Token, model.Password))
            .ReturnsAsync(IdentityResult.Failed(identityErrors));

        var (success, errors) = await _accountService.ResetPasswordAsync(model);

        Assert.That(success, Is.False);
        Assert.That(errors[0], Is.EqualTo("Invalid token"));
    }

    [Test]
    public async Task ResetPasswordAsync_UserNotFound_ReturnsFailure()
    {
        var model = new ResetPasswordViewModel
        {
            Email = "nonexisting@gruers.com",
            Token = "some-token",
            Password = "NewPass123!",
            ConfirmPassword = "NewPass123!"
        };

        _userManagerMock.Setup(x => x.FindByEmailAsync(model.Email))
            .ReturnsAsync((AppUser)null);

        var (success, errors) = await _accountService.ResetPasswordAsync(model);

        Assert.That(success, Is.False);
        Assert.That(errors[0], Is.EqualTo("User not found."));
    }

    #endregion ResetPasswordAsync Tests
}