using FluentValidation;

namespace DeveloperStore.Domain.Validation;

/// <summary>
/// Validator for sale number field.
/// </summary>
public class SaleNumberValidator : AbstractValidator<string>
{
    /// <summary>
    /// Initializes a new instance of SaleNumberValidator.
    /// </summary>
    public SaleNumberValidator()
    {
        RuleFor(saleNumber => saleNumber)
            .NotEmpty()
            .WithMessage("Sale number is required.")
            .MaximumLength(50)
            .WithMessage("Sale number must not exceed 50 characters.");
    }
}
