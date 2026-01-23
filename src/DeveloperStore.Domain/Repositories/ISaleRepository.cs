using DeveloperStore.Domain.Entities;
using DeveloperStore.Domain.Common;

namespace DeveloperStore.Domain.Repositories;

/// <summary>
/// Repository interface for Sale aggregate root.
/// </summary>
public interface ISaleRepository
{
    /// <summary>
    /// Gets a sale by its unique identifier.
    /// </summary>
    Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a sale by its sale number.
    /// </summary>
    Task<Sale?> GetBySaleNumberAsync(string saleNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the last sale number by prefix (used for sale number generation).
    /// </summary>
    Task<string?> GetLastSaleNumberByPrefixAsync(string prefix, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets paginated and filtered sales by customer ID.
    /// </summary>
    Task<PaginatedResult<Sale>> GetByCustomerIdAsync(
        Guid customerId,
        SaleQueryParameters parameters,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets paginated and filtered sales by branch ID.
    /// </summary>
    Task<PaginatedResult<Sale>> GetByBranchIdAsync(
        Guid branchId,
        SaleQueryParameters parameters,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets paginated and filtered sales within a date range.
    /// </summary>
    Task<PaginatedResult<Sale>> GetByDateRangeAsync(
        DateTime startDate,
        DateTime endDate,
        SaleQueryParameters parameters,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total amount for filtered sales by customer ID.
    /// </summary>
    Task<decimal> GetTotalAmountByCustomerIdAsync(
        Guid customerId,
        SaleQueryParameters parameters,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total amount for filtered sales by branch ID.
    /// </summary>
    Task<decimal> GetTotalAmountByBranchIdAsync(
        Guid branchId,
        SaleQueryParameters parameters,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total amount for filtered sales within a date range.
    /// </summary>
    Task<decimal> GetTotalAmountByDateRangeAsync(
        DateTime startDate,
        DateTime endDate,
        SaleQueryParameters parameters,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new sale.
    /// </summary>
    Task<Sale> CreateAsync(Sale sale, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing sale.
    /// </summary>
    Task<Sale> UpdateAsync(Sale sale, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a sale.
    /// </summary>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}


