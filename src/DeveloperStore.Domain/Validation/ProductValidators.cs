using FluentValidation;

namespace DeveloperStore.Domain.Validation;

/// <summary>
/// Validator for product title field.
/// </summary>
public class ProductTitleValidator : AbstractValidator<string>
{
    /// <summary>
    /// Initializes a new instance of ProductTitleValidator.
    /// </summary>
    public ProductTitleValidator()
    {
        RuleFor(title => title)
            .NotEmpty()
            .WithMessage("Product title is required.")
            .MaximumLength(200)
            .WithMessage("Product title must not exceed 200 characters.");
    }
}

/// <summary>
/// Validator for product category field.
/// </summary>
public class ProductCategoryValidator : AbstractValidator<string>
{
    /// <summary>
    /// Initializes a new instance of ProductCategoryValidator.
    /// </summary>
    public ProductCategoryValidator()
    {
        RuleFor(category => category)
            .NotEmpty()
            .WithMessage("Product category is required.")
            .MaximumLength(100)
            .WithMessage("Product category must not exceed 100 characters.");
    }
}

/// <summary>
/// Validator for product description field.
/// </summary>
public class ProductDescriptionValidator : AbstractValidator<string?>
{
    /// <summary>
    /// Initializes a new instance of ProductDescriptionValidator.
    /// </summary>
    public ProductDescriptionValidator()
    {
        RuleFor(description => description)
            .MaximumLength(500)
            .WithMessage("Product description must not exceed 500 characters.")
            .When(description => !string.IsNullOrEmpty(description));
    }
}
