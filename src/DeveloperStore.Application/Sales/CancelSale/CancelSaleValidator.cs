using FluentValidation;

namespace DeveloperStore.Application.Sales.CancelSale;

/// <summary>
/// Validator for CancelSaleCommand.
/// </summary>
public class CancelSaleValidator : AbstractValidator<CancelSaleCommand>
{
    public CancelSaleValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Sale ID is required");
    }
}
