
using DeveloperStore.Application.Sales.GetSale;

/// <summary>
/// Result containing sales for a customer.
/// </summary>
public class GetSalesByCustomerResult
{
    /// <summary>
    /// Gets or sets the customer ID.
    /// </summary>
    public Guid CustomerId { get; set; }

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
