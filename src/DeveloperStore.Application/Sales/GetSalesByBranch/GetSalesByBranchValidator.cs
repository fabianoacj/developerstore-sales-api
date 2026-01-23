using FluentValidation;

namespace DeveloperStore.Application.Sales.GetSalesByBranch;

/// <summary>
/// Validator for GetSalesByBranchCommand.
/// </summary>
public class GetSalesByBranchValidator : AbstractValidator<GetSalesByBranchCommand>
{
    /// <summary>
    /// Initializes a new instance of the GetSalesByBranchValidator.
    /// </summary>
    public GetSalesByBranchValidator()
    {
        RuleFor(x => x.BranchId)
            .NotEmpty()
            .WithMessage("Branch ID is required");
    }
}
