
using DeveloperStore.Application.Sales.GetSale;

/// <summary>
/// Result containing sales within a date range.
/// </summary>
public class GetSalesByDateRangeResult
{
    /// <summary>
    /// Gets or sets the start date.
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Gets or sets the end date.
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// Gets or sets the list of sales.
    /// </summary>
    public List<GetSaleResult> Sales { get; set; } = new();

    /// <summary>
    /// Gets or sets the total count of sales.
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Gets or sets the total amount across all sales.
    /// </summary>
    public decimal TotalAmount { get; set; }
}
