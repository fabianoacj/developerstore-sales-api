using FluentValidation;

namespace DeveloperStore.Application.Events.GetSaleEvents;

/// <summary>
/// Validator for GetSaleEventsQuery.
/// </summary>
public class GetSaleEventsValidator : AbstractValidator<GetSaleEventsQuery>
{
    /// <summary>
    /// Initializes a new instance of the GetSaleEventsValidator.
    /// </summary>
    public GetSaleEventsValidator()
    {
        RuleFor(x => x.SaleId)
            .NotEmpty()
            .WithMessage("Sale ID is required");
    }
}
