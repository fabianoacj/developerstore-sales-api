using FluentValidation;

namespace DeveloperStore.Domain.Validation;

/// <summary>
/// Validator for phone numbers.
/// </summary>
public class PhoneValidator : AbstractValidator<string?>
{
    /// <summary>
    /// Initializes a new instance of PhoneValidator.
    /// </summary>
    public PhoneValidator()
    {
        When(phone => !string.IsNullOrEmpty(phone), () =>
        {
            RuleFor(phone => phone)
                .MaximumLength(20)
                .WithMessage("Phone number must not exceed 20 characters.");
        });
    }
}

