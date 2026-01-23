using FluentValidation;

namespace DeveloperStore.Domain.Validation;

/// <summary>
/// Base validator for pagination parameters.
/// Enforces consistent pagination rules across the application.
/// </summary>
/// <typeparam name="T">The type being validated that has pagination properties.</typeparam>
public abstract class PaginationValidator<T> : AbstractValidator<T>
{
    /// <summary>
    /// Validates pagination parameters with customizable limits.
    /// </summary>
    /// <param name="pageSelector">Expression to select the page property.</param>
    /// <param name="pageSizeSelector">Expression to select the page size property.</param>
    /// <param name="maxPageSize">Maximum allowed page size (default: 500).</param>
    protected void ValidatePagination(
        System.Linq.Expressions.Expression<System.Func<T, int>> pageSelector,
        System.Linq.Expressions.Expression<System.Func<T, int>> pageSizeSelector,
        int maxPageSize = 500)
    {
        RuleFor(pageSelector)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page number must be 1 or greater");

        RuleFor(pageSizeSelector)
            .GreaterThan(0)
            .WithMessage("Page size must be greater than 0")
            .LessThanOrEqualTo(maxPageSize)
            .WithMessage($"Page size must not exceed {maxPageSize}");
    }
}

/// <summary>
/// Validator for skip/limit pagination (MongoDB-style).
/// </summary>
/// <typeparam name="T">The type being validated that has skip/limit properties.</typeparam>
public abstract class SkipLimitValidator<T> : AbstractValidator<T>
{
    /// <summary>
    /// Validates skip/limit pagination parameters.
    /// </summary>
    /// <param name="skipSelector">Expression to select the skip property.</param>
    /// <param name="limitSelector">Expression to select the limit property.</param>
    /// <param name="maxLimit">Maximum allowed limit (default: 500).</param>
    protected void ValidateSkipLimit(
        System.Linq.Expressions.Expression<System.Func<T, int>> skipSelector,
        System.Linq.Expressions.Expression<System.Func<T, int>> limitSelector,
        int maxLimit = 500)
    {
        RuleFor(skipSelector)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Skip must be 0 or greater");

        RuleFor(limitSelector)
            .GreaterThan(0)
            .WithMessage("Limit must be greater than 0")
            .LessThanOrEqualTo(maxLimit)
            .WithMessage($"Limit must not exceed {maxLimit}");
    }
}
