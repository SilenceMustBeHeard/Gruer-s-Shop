using GruersShop.Data.Models.Base;
using GruersShop.Services.Core.Service.Implementations.Account;
using GruersShop.Services.Core.Service.Interfaces.Account;
using GruersShop.Web.ViewModels.Account.Profile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GruersShop.Web.Controllers.Account;

public class AccountController : Controller
{
    private readonly IAccountService _accountService;
    private readonly UserManager<AppUser> _userManager;
    private readonly IEmailService _emailService;

    public AccountController(
        IAccountService accountService,
        UserManager<AppUser> userManager,
        IEmailService emailService)
    {
        _accountService = accountService;
        _userManager = userManager;
        _emailService = emailService;
    }

    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await _accountService.RegisterAsync(model);

        if (!result.Success)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error);
            return View(model);
        }

    
        var user = await _userManager.FindByEmailAsync(model.Email);

      
        var (tokenSuccess, tokenError, token) = await _accountService.GenerateEmailConfirmationAsync(user!);

        if (!tokenSuccess)
        {
            TempData["Error"] = tokenError ?? "Failed to send confirmation email";
            return RedirectToAction("Index", "Home");
        }

        var confirmationLink = Url.Action("ConfirmEmail", "Account",
            new { userId = user!.Id, token = token }, Request.Scheme);

        await _emailService.SendEmailAsync(
            user.Email!,
            "Confirm your email address",
            $"Please confirm your email by clicking this link: <a href='{confirmationLink}'>Confirm</a>"
        );

        return RedirectToAction("RegistrationConfirmation");
    }

    [HttpGet]
    public IActionResult RegistrationConfirmation() => View();

    [HttpGet]
    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
        var result = await _accountService.ConfirmEmailAsync(userId, token);

        if (!result.Success)
        {
            TempData["Error"] = result.Error;
            return RedirectToAction("Index", "Home");
        }

        TempData["Success"] = "Email confirmed successfully!";
        return RedirectToAction("Index", "Home");
    }
    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Invalid login attempt.";
            return View(model);
        }

        var success = await _accountService.LoginAsync(model);

        if (success)
            return RedirectToAction("Index", "Home");

        ModelState.AddModelError("", "Invalid login attempt.");
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _accountService.LogoutAsync();
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var resetLink = Url.Action("ResetPassword", "Account", null, Request.Scheme);

     
        await _accountService.ForgotPasswordAsync(model.Email, resetLink);

        TempData["Success"] = "If an account exists with this email, you will receive a password reset link.";
        return RedirectToAction(nameof(ForgotPasswordConfirmation));
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ResetPassword(string token, string email)
    {
        if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
        {
            TempData["Error"] = "Invalid password reset token.";
            return RedirectToAction(nameof(ForgotPassword));
        }

        var model = new ResetPasswordViewModel { Token = token, Email = email };
        return View(model);
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _accountService.ResetPasswordAsync(model);

        if (result.Success)
        {
            TempData["Success"] = "Your password has been reset successfully!";
            return RedirectToAction(nameof(ResetPasswordConfirmation));
        }

        foreach (var error in result.Errors)
            ModelState.AddModelError(string.Empty, error);

        return View(model);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ForgotPasswordConfirmation()
    {
        return View();
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ResetPasswordConfirmation()
    {
        return View();
    }
}