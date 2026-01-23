using FluentValidation;

namespace DeveloperStore.Application.Sales.GetSalesByDateRange;

/// <summary>
/// Validator for GetSalesByDateRangeCommand.
/// </summary>
public class GetSalesByDateRangeValidator : AbstractValidator<GetSalesByDateRangeCommand>
{
    /// <summary>
    /// Initializes a new instance of the GetSalesByDateRangeValidator.
    /// </summary>
    public GetSalesByDateRangeValidator()
    {
        RuleFor(x => x.StartDate)
            .NotEmpty()
            .WithMessage("Start date is required")
            .LessThanOrEqualTo(x => x.EndDate)
            .WithMessage("Start date must be less than or equal to end date");

        RuleFor(x => x.EndDate)
            .NotEmpty()
            .WithMessage("End date is required")
            .GreaterThanOrEqualTo(x => x.StartDate)
            .WithMessage("End date must be greater than or equal to start date");

        RuleFor(x => x)
            .Must(x => (x.EndDate - x.StartDate).TotalDays <= 365)
            .WithMessage("Date range cannot exceed 365 days");
    }
}
