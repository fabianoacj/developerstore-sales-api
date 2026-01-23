using DeveloperStore.Domain.Validation;
using FluentValidation;

namespace DeveloperStore.Application.Sales.GetSales;

/// <summary>
/// Validator for GetSalesQuery that validates pagination parameters.
/// </summary>
public class GetSalesValidator : PaginationValidator<GetSalesQuery>
{
    /// <summary>
    /// Initializes a new instance of the GetSalesValidator with pagination validation rules.
    /// </summary>
    public GetSalesValidator()
    {
        ValidatePagination(x => x.Page, x => x.PageSize, maxPageSize: 500);
    }
}
