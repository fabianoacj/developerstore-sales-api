using DeveloperStore.Domain.Entities;

namespace DeveloperStore.Domain.Repositories;

/// <summary>
/// Repository interface for Sale aggregate root.
/// </summary>
public interface ISaleRepository
{
    Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Sale?> GetBySaleNumberAsync(string saleNumber, CancellationToken cancellationToken = default);

    Task<IEnumerable<Sale>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<IEnumerable<Sale>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);

    Task<IEnumerable<Sale>> GetByBranchIdAsync(Guid branchId, CancellationToken cancellationToken = default);

    Task<IEnumerable<Sale>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

    Task<Sale> CreateAsync(Sale sale, CancellationToken cancellationToken = default);

    Task<Sale> UpdateAsync(Sale sale, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
