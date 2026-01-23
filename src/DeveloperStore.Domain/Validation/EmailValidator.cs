using FluentValidation;
using System.Text.RegularExpressions;

namespace DeveloperStore.Domain.Validation;

/// <summary>
/// Validator for email addresses following RFC standards.
/// </summary>
public class EmailValidator : AbstractValidator<string>
{
    /// <summary>
    /// Initializes a new instance of EmailValidator.
    /// </summary>
    public EmailValidator()
    {
        RuleFor(email => email)
            .NotEmpty()
            .WithMessage("The email address cannot be empty.")
            .MaximumLength(100)
            .WithMessage("The email address cannot be longer than 100 characters.")
            .Must(BeValidEmail)
            .WithMessage("The provided email address is not valid.");
    }

    private bool BeValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        // RFC 5322 compliant email validation
        var regex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
        return regex.IsMatch(email);
    }
}
