namespace DeveloperStore.WebApi.Features.Sales.UpdateSale;

/// <summary>
/// Request model for updating a sale.
/// </summary>
public class UpdateSaleRequest
{
    /// <summary>
    /// Gets or sets the customer name.
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the customer email.
    /// </summary>
    public string CustomerEmail { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the customer phone.
    /// </summary>
    public string? CustomerPhone { get; set; }

    /// <summary>
    /// Gets or sets the list of items.
    /// </summary>
    public List<UpdateSaleItemRequest> Items { get; set; } = new();
}

/// <summary>
/// Request model for updating a sale item.
/// </summary>
public class UpdateSaleItemRequest
{
    /// <summary>
    /// Gets or sets the item ID (Guid.Empty for new items).
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the product ID.
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// Gets or sets the product title.
    /// </summary>
    public string ProductTitle { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the product category.
    /// </summary>
    public string ProductCategory { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the product description.
    /// </summary>
    public string? ProductDescription { get; set; }

    /// <summary>
    /// Gets or sets the quantity.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the unit price.
    /// </summary>
    public decimal UnitPrice { get; set; }
}
