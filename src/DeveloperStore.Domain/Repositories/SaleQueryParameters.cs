using DeveloperStore.Domain.Enums;

namespace DeveloperStore.Domain.Repositories;

/// <summary>
/// Query parameters for filtering, sorting, and paginating sales.
/// </summary>
public class SaleQueryParameters
{
    /// <summary>
    /// Gets or sets the page number (1-based).
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Gets or sets the page size.
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Gets or sets the order specification (e.g., "SaleDate desc, TotalAmount asc").
    /// </summary>
    public string? OrderBy { get; set; }

    // String filters (support partial matching with *)
    public string? SaleNumber { get; set; }
    public string? CustomerName { get; set; }
    public string? BranchName { get; set; }

    // Date range filters
    public DateTime? MinSaleDate { get; set; }
    public DateTime? MaxSaleDate { get; set; }

    // Numeric range filters
    public decimal? MinTotalAmount { get; set; }
    public decimal? MaxTotalAmount { get; set; }

    // Status filter
    public SaleStatus? Status { get; set; }
}
