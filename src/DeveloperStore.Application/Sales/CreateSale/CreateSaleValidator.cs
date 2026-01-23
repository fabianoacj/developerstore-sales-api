using FluentValidation;
using DeveloperStore.Domain.Validation;

namespace DeveloperStore.Application.Sales.CreateSale;

/// <summary>
/// Validator for CreateSaleCommand that defines validation rules for sale creation.
/// </summary>
public class CreateSaleValidator : AbstractValidator<CreateSaleCommand>
{
    /// <summary>
    /// Initializes a new instance of the CreateSaleValidator with defined validation rules.
    /// </summary>
    /// <remarks>
    /// Validation rules include:
    /// - SaleNumber: Using Domain SaleNumberValidator
    /// - SaleDate: Required, cannot be in the future
    /// - CustomerId: Must be a valid GUID (not empty)
    /// - CustomerName: Using Domain NameValidator
    /// - CustomerEmail: Using Domain EmailValidator
    /// - CustomerPhone: Using Domain PhoneValidator
    /// - BranchId: Must be a valid GUID (not empty)
    /// - BranchName: Using Domain NameValidator
    /// - Items: Must have at least one item, each validated with domain validators
    /// </remarks>
    public CreateSaleValidator()
    {
        // Using Domain SaleNumberValidator
        RuleFor(x => x.SaleNumber)
            .SetValidator(new SaleNumberValidator());

        RuleFor(x => x.SaleDate)
            .NotEmpty()
            .WithMessage("Sale date is required")
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("Sale date cannot be in the future");

        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("Customer ID is required");

        // Using Domain NameValidator for CustomerName
        RuleFor(x => x.CustomerName)
            .SetValidator(new NameValidator());

        // Using Domain EmailValidator
        RuleFor(x => x.CustomerEmail)
            .SetValidator(new EmailValidator());

        // Using Domain PhoneValidator (handles nullable)
        RuleFor(x => x.CustomerPhone)
            .SetValidator(new PhoneValidator()!);

        RuleFor(x => x.BranchId)
            .NotEmpty()
            .WithMessage("Branch ID is required");

        // Using Domain NameValidator for BranchName
        RuleFor(x => x.BranchName)
            .SetValidator(new NameValidator());

        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("Sale must have at least one item")
            .Must(items => items != null && items.Count > 0)
            .WithMessage("Sale must have at least one item");

        RuleForEach(x => x.Items)
            .SetValidator(new CreateSaleItemValidator());
    }
}

/// <summary>
/// Validator for CreateSaleItemDto.
/// </summary>
public class CreateSaleItemValidator : AbstractValidator<CreateSaleItemDto>
{
    public CreateSaleItemValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Product ID is required");

        // Using Domain ProductTitleValidator
        RuleFor(x => x.ProductTitle)
            .SetValidator(new ProductTitleValidator());

        // Using Domain ProductCategoryValidator
        RuleFor(x => x.ProductCategory)
            .SetValidator(new ProductCategoryValidator());

        // Using Domain ProductDescriptionValidator
        RuleFor(x => x.ProductDescription)
            .SetValidator(new ProductDescriptionValidator()!);

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than zero")
            .LessThanOrEqualTo(20)
            .WithMessage("It's not possible to sell above 20 identical items");

        RuleFor(x => x.UnitPrice)
            .GreaterThan(0)
            .WithMessage("Unit price must be greater than zero");
    }
}
