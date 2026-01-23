using FluentValidation;
using DeveloperStore.Domain.Validation;

namespace DeveloperStore.Application.Sales.UpdateSale;

/// <summary>
/// Validator for UpdateSaleCommand.
/// </summary>
public class UpdateSaleValidator : AbstractValidator<UpdateSaleCommand>
{
    /// <summary>
    /// Initializes a new instance of the UpdateSaleValidator.
    /// </summary>
    public UpdateSaleValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Sale ID is required");

        RuleFor(x => x.SaleDate)
            .NotEmpty()
            .WithMessage("Sale date is required")
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("Sale date cannot be in the future");

        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("Sale must have at least one item")
            .Must(items => items != null && items.Count > 0)
            .WithMessage("Sale must have at least one item");

        RuleForEach(x => x.Items)
            .SetValidator(new UpdateSaleItemValidator());
    }
}

/// <summary>
/// Validator for UpdateSaleItemDto.
/// </summary>
public class UpdateSaleItemValidator : AbstractValidator<UpdateSaleItemDto>
{
    public UpdateSaleItemValidator()
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
