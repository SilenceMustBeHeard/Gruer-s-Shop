using FluentValidation;
using GruersShop.Data.Models.Messages;

namespace GruersShop.Services.Common.Validators;

public class ContactMessageValidator : AbstractValidator<ContactMessage>
{
    public ContactMessageValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required.");

        RuleFor(x => x.Subject)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(100);

        RuleFor(x => x.Message)
            .NotEmpty()
            .MinimumLength(10)
            .MaximumLength(5000);

        RuleFor(x => x.Response)
            .MaximumLength(5000)
            .When(x => x.Response is not null);
    }
}