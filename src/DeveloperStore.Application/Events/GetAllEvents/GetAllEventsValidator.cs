using DeveloperStore.Domain.Validation;
using FluentValidation;

namespace DeveloperStore.Application.Events.GetAllEvents;

/// <summary>
/// Validator for GetAllEventsQuery that validates pagination parameters.
/// </summary>
public class GetAllEventsValidator : SkipLimitValidator<GetAllEventsQuery>
{
    /// <summary>
    /// Initializes a new instance of the GetAllEventsValidator with pagination validation rules.
    /// </summary>
    public GetAllEventsValidator()
    {
        ValidateSkipLimit(x => x.Skip, x => x.Limit, maxLimit: 500);
    }
}
