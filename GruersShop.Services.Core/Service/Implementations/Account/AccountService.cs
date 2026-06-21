using GruersShop.Data.Models.Base;
using GruersShop.Services.Core.Service.Interfaces.Account;
using GruersShop.Web.ViewModels.Account.Profile;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;

namespace GruersShop.Services.Core.Service.Implementations.Account;

public class AccountService : IAccountService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IEmailService _emailService;
    private readonly IViewRenderService _viewRenderService;
    private readonly ILogger<AccountService> _logger;

    public AccountService(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        IEmailService emailService,
        IViewRenderService viewRenderService,
        ILogger<AccountService> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailService = emailService;
        _viewRenderService = viewRenderService;
        _logger = logger;
    }

    // Returns a tuple indicating success and any error messages
    // registers a new user, assigns them the "User" ( by default) role, and signs them in
    public async Task<(bool Success, string[] Errors)> RegisterAsync(RegisterViewModel model)
    {
        var user = new AppUser
        {
            UserName = model.Email,
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName,
            Address = model.Address,
            AlternateEmail = model.AlternateEmail
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "User");
            await _signInManager.SignInAsync(user, isPersistent: false);
            return (true, Array.Empty<string>());
        }

        return (false, result.Errors.Select(e => e.Description).ToArray());
    }

    // Attempts to sign in a user with the provided credentials and returns whether the login was successful
    public async Task<bool> LoginAsync(LoginViewModel model)
    {
        var result = await _signInManager.PasswordSignInAsync(
            model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

        return result.Succeeded;
    }

    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
    }

    // Handles the forgot password process by generating a reset token
    // creating a reset link, rendering an email template, and sending the email to the user
    //
    public async Task<bool> ForgotPasswordAsync(string email, string resetLink)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) return false;

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var fullResetLink = $"{resetLink}?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(email)}";

        var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());
        viewData["ResetLink"] = fullResetLink;

        var emailBody = await _viewRenderService.RenderToStringAsync("Emails/PasswordReset", user, viewData);

        return await _emailService.SendEmailAsync(email, "Password Reset Request", emailBody);
    }

    // Resets the user's password using the provided token and new password,
    // returning success status and any error messages
    // If the user is not found, it returns an error message indicating that the user was not found

    public async Task<(bool Success, string[] Errors)> ResetPasswordAsync(ResetPasswordViewModel model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return (false, new[] { "User not found." });
        }

        var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);

        if (result.Succeeded)
        {
            return (true, Array.Empty<string>());
        }

        return (false, result.Errors.Select(e => e.Description).ToArray());
    }

    public async Task<(bool Success, string? Error, string? ConfirmationLink)> GenerateEmailConfirmationAsync(AppUser user)
    {
        try
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            return (true, null, token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate email confirmation token for user {UserId}", user.Id);
            return (false, "Failed to generate confirmation token", null);
        }
    }

    // Confirms the user's email using the provided user ID and token,
    // returning success status, any error messages, and the user object if successful
    public async Task<(bool Success, string? Error, AppUser? User)> ConfirmEmailAsync(string userId, string token)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            return (false, "Invalid confirmation link", null);

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return (false, "User not found", null);

        var result = await _userManager.ConfirmEmailAsync(user, token);

        if (result.Succeeded)
        {
            // Optionally sign in the user after confirming their email
            await _signInManager.SignInAsync(user, isPersistent: false);
            _logger.LogInformation("Email confirmed for user {UserId}", userId);
            return (true, null, user);
        }

        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
        _logger.LogWarning("Email confirmation failed for user {UserId}: {Errors}", userId, errors);
        return (false, "Email confirmation failed", null);
    }
}