using FluentValidation;

namespace DeveloperStore.Domain.Validation;

/// <summary>
/// Validator for name fields (customer name, branch name, etc.) with max 200 characters.
/// </summary>
public class NameValidator : AbstractValidator<string>
{
    /// <summary>
    /// Initializes a new instance of NameValidator.
    /// </summary>
    public NameValidator()
    {
        RuleFor(name => name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MaximumLength(200)
            .WithMessage("Name must not exceed 200 characters.");
    }
}
