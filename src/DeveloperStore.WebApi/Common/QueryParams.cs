namespace DeveloperStore.WebApi.Common;

/// <summary>
/// Base class for paginated query parameters.
/// </summary>
public class PaginatedQueryParams
{
    private int page = 1;
    private int size = 10;

    public int _page
    {
        get => page;
        set => page = value < 1 ? 1 : value;
    }

    public int _size
    {
        get => size;
        set => size = value < 1 ? 10 : (value > 100 ? 100 : value);
    }

    public string? _order { get; set; }

    /// <summary>
    /// Calculated skip value for database queries (internal use only, not exposed in API).
    /// </summary>
    internal int Skip => (page - 1) * size;

    /// <summary>
    /// Calculated take value for database queries (internal use only, not exposed in API).
    /// </summary>
    internal int Take => size;
}

/// <summary>
/// Consolidated query parameters for the main sales endpoint.
/// Supports filtering by customer, branch, dates, amounts, and status.
/// </summary>
public class SalesQueryParams : PaginatedQueryParams
{
    // Filter by IDs
    /// <summary>
    /// Filter by customer ID.
    /// </summary>
    public Guid? customerId { get; set; }

    /// <summary>
    /// Filter by branch ID.
    /// </summary>
    public Guid? branchId { get; set; }

    // Filter by properties
    /// <summary>
    /// Filter by sale number (supports partial match with *).
    /// </summary>
    public string? saleNumber { get; set; }

    /// <summary>
    /// Filter by customer name (supports partial match with *).
    /// </summary>
    public string? customerName { get; set; }

    /// <summary>
    /// Filter by branch name (supports partial match with *).
    /// </summary>
    public string? branchName { get; set; }

    // Date range filters
    /// <summary>
    /// Minimum sale date.
    /// </summary>
    public DateTime? _minSaleDate { get; set; }

    /// <summary>
    /// Maximum sale date.
    /// </summary>
    public DateTime? _maxSaleDate { get; set; }

    // Amount range filters
    /// <summary>
    /// Minimum total amount.
    /// </summary>
    public decimal? _minTotalAmount { get; set; }

    /// <summary>
    /// Maximum total amount.
    /// </summary>
    public decimal? _maxTotalAmount { get; set; }

    /// <summary>
    /// Filter by status (Active, Cancelled).
    /// </summary>
    public string? status { get; set; }
}


