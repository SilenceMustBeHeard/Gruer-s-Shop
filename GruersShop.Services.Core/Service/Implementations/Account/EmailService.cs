using GruersShop.Services.Common.Validators;
using GruersShop.Services.Core.Service.Interfaces.Account;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace GruersShop.Services.Core.Service.Implementations.Account;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;
    private readonly EmailValidator _emailValidator;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
        _emailValidator = new EmailValidator();
    }


    // sends an email using SendGrid API
    // It validates the email address, subject, and body before sending
    // It also logs the process and handles any exceptions that may occur during the email sending process.
    public async Task<bool> SendEmailAsync(string to, string subject, string body)
    {

        if (!_emailValidator.IsValidEmail(to) ||
            string.IsNullOrWhiteSpace(subject) ||
            string.IsNullOrWhiteSpace(body))
        {
            _logger.LogWarning("Invalid email request parameters");
            return false;
        }

        try
        {
            var apiKey = _configuration["SendGrid:ApiKey"];
            var fromEmail = _configuration["SendGrid:FromEmail"];
            var fromName = _configuration["SendGrid:FromName"];

            if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(fromEmail))
            {
                _logger.LogError("Email service configuration error");
                return false;
            }

            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(fromEmail, fromName ?? "Service");
            var toEmail = new EmailAddress(_emailValidator.NormalizeEmail(to));

            var plainTextContent = StripHtml(body);

            var msg = MailHelper.CreateSingleEmail(
                from,
                toEmail,
                subject,
                plainTextContent,
                body
            );

            var response = await client.SendEmailAsync(msg);


            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Email service request completed");
                return true;
            }

            _logger.LogWarning("Email service request failed");
            return false;
        }
        catch (Exception ex)
        {

            _logger.LogWarning(ex, "Email service error");
            return false;
        }
    }

    private static string StripHtml(string html)
    {
        if (string.IsNullOrWhiteSpace(html))
            return string.Empty;

        return System.Text.RegularExpressions.Regex.Replace(html, "<.*?>", string.Empty);
    }
}
