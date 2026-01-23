namespace DeveloperStore.Application.Sales.CancelSaleItem;

/// <summary>
/// Represents the response returned after successfully cancelling a sale item.
/// </summary>
public class CancelSaleItemResult
{
    public Guid SaleId { get; set; }

    public Guid ItemId { get; set; }

    public string SaleNumber { get; set; } = string.Empty;

    public bool Success { get; set; }

    public string Message { get; set; } = string.Empty;
}
