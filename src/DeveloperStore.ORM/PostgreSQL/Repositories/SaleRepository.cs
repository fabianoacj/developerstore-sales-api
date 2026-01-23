using DeveloperStore.Domain.Entities;
using DeveloperStore.Domain.Repositories;
using DeveloperStore.Domain.Common;
using DeveloperStore.ORM.PostgreSQL.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DeveloperStore.ORM.PostgreSQL.Repositories;

/// <summary>
/// Implementation of ISaleRepository using Entity Framework Core and PostgreSQL.
/// </summary>
public class SaleRepository : ISaleRepository
{
    private readonly DefaultContext _context;
    public SaleRepository(DefaultContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves a sale by its unique identifier, including all items.
    /// </summary>
    /// <param name="id">The unique identifier of the sale.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The sale if found, null otherwise.</returns>
    public async Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var sale = await _context.Sales
            .Include("_items")
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        return sale;
    }

    /// <summary>
    /// Retrieves a sale by its sale number, including all items.
    /// </summary>
    /// <param name="saleNumber">The sale number to search for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The sale if found, null otherwise.</returns>
    public async Task<Sale?> GetBySaleNumberAsync(string saleNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include("_items")
            .FirstOrDefaultAsync(s => s.SaleNumber == saleNumber, cancellationToken);
    }

    /// <summary>
    /// Gets the last sale number by prefix (used for sale number generation).
    /// </summary>
    /// <param name="prefix">The prefix to search for (e.g., "SALE-20260123").</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The last sale number with the given prefix, or null if none found.</returns>
    public async Task<string?> GetLastSaleNumberByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Where(s => s.SaleNumber.StartsWith(prefix))
            .OrderByDescending(s => s.SaleNumber)
            .Select(s => s.SaleNumber)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Gets paginated and filtered sales by customer ID.
    /// </summary>
    public async Task<PaginatedResult<Sale>> GetByCustomerIdAsync(
        Guid customerId,
        SaleQueryParameters parameters,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Sales
            .Include("_items")
            .Where(s => s.Customer.Id == customerId);

        query = ApplyFilters(query, parameters);
        query = ApplyOrdering(query, parameters.OrderBy);

        return await CreatePaginatedResultAsync(query, parameters.Page, parameters.PageSize, parameters, cancellationToken);
    }

    /// <summary>
    /// Gets paginated and filtered sales by branch ID.
    /// </summary>
    public async Task<PaginatedResult<Sale>> GetByBranchIdAsync(
        Guid branchId,
        SaleQueryParameters parameters,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Sales
            .Include("_items")
            .Where(s => s.Branch.Id == branchId);

        query = ApplyFilters(query, parameters);
        query = ApplyOrdering(query, parameters.OrderBy);

        return await CreatePaginatedResultAsync(query, parameters.Page, parameters.PageSize, parameters, cancellationToken);
    }

    /// <summary>
    /// Gets paginated and filtered sales within a date range.
    /// </summary>
    public async Task<PaginatedResult<Sale>> GetByDateRangeAsync(
        DateTime startDate,
        DateTime endDate,
        SaleQueryParameters parameters,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Sales
            .Include("_items")
            .Where(s => s.SaleDate >= startDate && s.SaleDate <= endDate);

        query = ApplyFilters(query, parameters);
        query = ApplyOrdering(query, parameters.OrderBy);

        return await CreatePaginatedResultAsync(query, parameters.Page, parameters.PageSize, parameters, cancellationToken);
    }

    /// <summary>
    /// Gets the total amount for filtered sales by customer ID.
    /// </summary>
    public async Task<decimal> GetTotalAmountByCustomerIdAsync(
        Guid customerId,
        SaleQueryParameters parameters,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Sales
            .Include("_items")
            .Where(s => s.Customer.Id == customerId);

        query = ApplyFilters(query, parameters);
        var sales = await query.ToListAsync(cancellationToken);
        return sales.Sum(s => s.TotalAmount);
    }

    /// <summary>
    /// Gets the total amount for filtered sales by branch ID.
    /// </summary>
    public async Task<decimal> GetTotalAmountByBranchIdAsync(
        Guid branchId,
        SaleQueryParameters parameters,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Sales
            .Include("_items")
            .Where(s => s.Branch.Id == branchId);

        query = ApplyFilters(query, parameters);
        var sales = await query.ToListAsync(cancellationToken);
        return sales.Sum(s => s.TotalAmount);
    }

    /// <summary>
    /// Gets the total amount for filtered sales within a date range.
    /// </summary>
    public async Task<decimal> GetTotalAmountByDateRangeAsync(
        DateTime startDate,
        DateTime endDate,
        SaleQueryParameters parameters,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Sales
            .Include("_items")
            .Where(s => s.SaleDate >= startDate && s.SaleDate <= endDate);

        query = ApplyFilters(query, parameters);
        var sales = await query.ToListAsync(cancellationToken);
        return sales.Sum(s => s.TotalAmount);
    }

    /// <summary>
    /// Applies filters from query parameters to the query.
    /// </summary>
    private static IQueryable<Sale> ApplyFilters(IQueryable<Sale> query, SaleQueryParameters parameters)
    {
        // Sale number filter
        if (!string.IsNullOrEmpty(parameters.SaleNumber))
        {
            if (parameters.SaleNumber.StartsWith("*") && parameters.SaleNumber.EndsWith("*"))
                query = query.Where(s => s.SaleNumber.Contains(parameters.SaleNumber.Trim('*')));
            else if (parameters.SaleNumber.StartsWith("*"))
                query = query.Where(s => s.SaleNumber.EndsWith(parameters.SaleNumber.TrimStart('*')));
            else if (parameters.SaleNumber.EndsWith("*"))
                query = query.Where(s => s.SaleNumber.StartsWith(parameters.SaleNumber.TrimEnd('*')));
            else
                query = query.Where(s => s.SaleNumber == parameters.SaleNumber);
        }

        // Customer name filter
        if (!string.IsNullOrEmpty(parameters.CustomerName))
        {
            if (parameters.CustomerName.StartsWith("*") && parameters.CustomerName.EndsWith("*"))
                query = query.Where(s => s.Customer.Name.Contains(parameters.CustomerName.Trim('*')));
            else if (parameters.CustomerName.StartsWith("*"))
                query = query.Where(s => s.Customer.Name.EndsWith(parameters.CustomerName.TrimStart('*')));
            else if (parameters.CustomerName.EndsWith("*"))
                query = query.Where(s => s.Customer.Name.StartsWith(parameters.CustomerName.TrimEnd('*')));
            else
                query = query.Where(s => s.Customer.Name == parameters.CustomerName);
        }

        // Branch name filter
        if (!string.IsNullOrEmpty(parameters.BranchName))
        {
            if (parameters.BranchName.StartsWith("*") && parameters.BranchName.EndsWith("*"))
                query = query.Where(s => s.Branch.Name.Contains(parameters.BranchName.Trim('*')));
            else if (parameters.BranchName.StartsWith("*"))
                query = query.Where(s => s.Branch.Name.EndsWith(parameters.BranchName.TrimStart('*')));
            else if (parameters.BranchName.EndsWith("*"))
                query = query.Where(s => s.Branch.Name.StartsWith(parameters.BranchName.TrimEnd('*')));
            else
                query = query.Where(s => s.Branch.Name == parameters.BranchName);
        }

        // Date range filters
        if (parameters.MinSaleDate.HasValue)
            query = query.Where(s => s.SaleDate >= parameters.MinSaleDate.Value);

        if (parameters.MaxSaleDate.HasValue)
            query = query.Where(s => s.SaleDate <= parameters.MaxSaleDate.Value);

        // Status filter
        if (parameters.Status.HasValue)
            query = query.Where(s => s.Status == parameters.Status.Value);

        // Note: TotalAmount filters cannot be applied here as TotalAmount is a computed property
        // They must be applied in-memory after loading the data

        return query;
    }

    /// <summary>
    /// Applies TotalAmount filters in memory (cannot be translated to SQL).
    /// </summary>
    private static IEnumerable<Sale> ApplyTotalAmountFilters(IEnumerable<Sale> sales, SaleQueryParameters parameters)
    {
        // Amount range filters - applied in memory since TotalAmount is computed
        if (parameters.MinTotalAmount.HasValue)
            sales = sales.Where(s => s.TotalAmount >= parameters.MinTotalAmount.Value);

        if (parameters.MaxTotalAmount.HasValue)
            sales = sales.Where(s => s.TotalAmount <= parameters.MaxTotalAmount.Value);

        return sales;
    }

    /// <summary>
    /// Applies ordering to the query based on order specification.
    /// </summary>
    private static IQueryable<Sale> ApplyOrdering(IQueryable<Sale> query, string? orderBy)
    {
        if (string.IsNullOrWhiteSpace(orderBy))
            return query;

        SaleSortFields.ValidateOrderBy(orderBy);

        var orderClauses = orderBy
            .Split(',')
            .Select(x => x.Trim().Trim('"'))
            .Where(x => !string.IsNullOrEmpty(x))
            .ToList();

        IOrderedQueryable<Sale>? orderedQuery = null;

        foreach (var clause in orderClauses)
        {
            var parts = clause.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var propertyName = parts[0];
            var isDescending = parts.Length > 1 &&
                parts[1].Equals("desc", StringComparison.OrdinalIgnoreCase);

            var property = typeof(Sale).GetProperty(
                propertyName,
                System.Reflection.BindingFlags.IgnoreCase |
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.Instance);

            if (property == null)
                continue;

            var parameter = Expression.Parameter(typeof(Sale), "x");
            var propertyAccess = Expression.Property(parameter, property);
            var lambda = Expression.Lambda(propertyAccess, parameter);

            var methodName = orderedQuery == null
                ? (isDescending ? "OrderByDescending" : "OrderBy")
                : (isDescending ? "ThenByDescending" : "ThenBy");

            var method = typeof(Queryable).GetMethods()
                .First(m => m.Name == methodName && m.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(Sale), property.PropertyType);

            orderedQuery = (IOrderedQueryable<Sale>)method.Invoke(null, new object[] { orderedQuery ?? query, lambda })!;
        }

        return orderedQuery ?? query;
    }

    /// <summary>
    /// Creates a paginated result from a query.
    /// </summary>
    private static async Task<PaginatedResult<Sale>> CreatePaginatedResultAsync(
        IQueryable<Sale> query,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<Sale>
        {
            Items = items,
            CurrentPage = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages
        };
    }

    /// <summary>
    /// Creates a paginated result with TotalAmount filtering applied in memory.
    /// </summary>
    private static async Task<PaginatedResult<Sale>> CreatePaginatedResultAsync(
        IQueryable<Sale> query,
        int page,
        int pageSize,
        SaleQueryParameters parameters,
        CancellationToken cancellationToken)
    {
        // Load all matching data (after applying SQL filters)
        var allSales = await query.ToListAsync(cancellationToken);

        // Apply TotalAmount filters in memory
        var filteredSales = ApplyTotalAmountFilters(allSales, parameters).ToList();

        // Calculate pagination
        var totalCount = filteredSales.Count;
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        // Apply pagination
        var items = filteredSales
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PaginatedResult<Sale>
        {
            Items = items,
            CurrentPage = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages
        };
    }

    /// <summary>
    /// Creates a new sale in the database.
    /// </summary>
    /// <param name="sale">The sale to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created sale.</returns>
    public async Task<Sale> CreateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        await _context.Sales.AddAsync(sale, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return sale;
    }

    /// <summary>
    /// Updates an existing sale in the database.
    /// </summary>
    /// <param name="sale">The sale to update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated sale.</returns>
    public async Task<Sale> UpdateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        _context.Sales.Update(sale);
        await _context.SaveChangesAsync(cancellationToken);
        return sale;
    }

    /// <summary>
    /// Deletes a sale from the database.
    /// </summary>
    /// <param name="id">The unique identifier of the sale to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the sale was deleted, false if not found.</returns>
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var sale = await GetByIdAsync(id, cancellationToken);
        if (sale == null)
            return false;

        _context.Sales.Remove(sale);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
