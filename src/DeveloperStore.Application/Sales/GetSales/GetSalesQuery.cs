using DeveloperStore.Domain.Common;
using DeveloperStore.Domain.Enums;
using MediatR;

namespace DeveloperStore.Application.Sales.GetSales;

/// <summary>
/// Query to retrieve sales with flexible filtering, pagination, and sorting.
/// </summary>
public class GetSalesQuery : IRequest<GetSalesResult>
{
    /// <summary>
    /// Gets or sets the page number for pagination.
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Gets or sets the page size for pagination.
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Gets or sets the order by clause for sorting.
    /// </summary>
    public string? OrderBy { get; set; }

    // Filter by identifiers
    /// <summary>
    /// Gets or sets the customer ID filter.
    /// </summary>
    public Guid? CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the branch ID filter.
    /// </summary>
    public Guid? BranchId { get; set; }

    // Filter by properties
    /// <summary>
    /// Gets or sets the sale number filter (supports wildcards).
    /// </summary>
    public string? SaleNumber { get; set; }

    /// <summary>
    /// Gets or sets the customer name filter (supports wildcards).
    /// </summary>
    public string? CustomerName { get; set; }

    /// <summary>
    /// Gets or sets the branch name filter (supports wildcards).
    /// </summary>
    public string? BranchName { get; set; }

    // Date range filters
    /// <summary>
    /// Gets or sets the minimum sale date filter.
    /// </summary>
    public DateTime? MinSaleDate { get; set; }

    /// <summary>
    /// Gets or sets the maximum sale date filter.
    /// </summary>
    public DateTime? MaxSaleDate { get; set; }

    // Amount range filters
    /// <summary>
    /// Gets or sets the minimum total amount filter.
    /// </summary>
    public decimal? MinTotalAmount { get; set; }

    /// <summary>
    /// Gets or sets the maximum total amount filter.
    /// </summary>
    public decimal? MaxTotalAmount { get; set; }

    // Status filter
    /// <summary>
    /// Gets or sets the sale status filter.
    /// </summary>
    public SaleStatus? Status { get; set; }
}
