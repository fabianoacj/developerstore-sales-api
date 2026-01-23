using DeveloperStore.Application.Sales.GetSale;

/// <summary>
/// Result containing sales for a branch.
/// </summary>
public class GetSalesByBranchResult
{
    /// <summary>
    /// Gets or sets the branch ID.
    /// </summary>
    public Guid BranchId { get; set; }

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
