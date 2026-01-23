namespace DeveloperStore.Application.Sales.CancelSale;

/// <summary>
/// Represents the response returned after successfully cancelling a sale.
/// </summary>
public class CancelSaleResult
{
    public Guid Id { get; set; }

    public string SaleNumber { get; set; } = string.Empty;

    public bool Success { get; set; }

    public string Message { get; set; } = string.Empty;
}
