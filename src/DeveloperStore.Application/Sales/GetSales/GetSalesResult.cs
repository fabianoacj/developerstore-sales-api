using DeveloperStore.Application.Sales.GetSale;

namespace DeveloperStore.Application.Sales.GetSales;

/// <summary>
/// Result for GetSales query containing paginated sales data.
/// </summary>
public class GetSalesResult
{
    /// <summary>
    /// Gets or sets the list of sales.
    /// </summary>
    public List<GetSaleResult> Items { get; set; } = new();

    /// <summary>
    /// Gets or sets the current page number.
    /// </summary>
    public int CurrentPage { get; set; }

    /// <summary>
    /// Gets or sets the page size.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Gets or sets the total count of items.
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Gets or sets the total number of pages.
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// Gets whether there is a previous page.
    /// </summary>
    public bool HasPreviousPage => CurrentPage > 1;

    /// <summary>
    /// Gets whether there is a next page.
    /// </summary>
    public bool HasNextPage => CurrentPage < TotalPages;
}
