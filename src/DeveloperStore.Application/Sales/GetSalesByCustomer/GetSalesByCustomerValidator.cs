using FluentValidation;

namespace DeveloperStore.Application.Sales.GetSalesByCustomer;

/// <summary>
/// Validator for GetSalesByCustomerCommand.
/// </summary>
public class GetSalesByCustomerValidator : AbstractValidator<GetSalesByCustomerCommand>
{
    /// <summary>
    /// Initializes a new instance of the GetSalesByCustomerValidator.
    /// </summary>
    public GetSalesByCustomerValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("Customer ID is required");
    }
}
