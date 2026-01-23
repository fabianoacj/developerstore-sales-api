using AutoMapper;
using DeveloperStore.Domain.Common;
using DeveloperStore.Domain.Repositories;
using MediatR;

namespace DeveloperStore.Application.Sales.GetSales;

/// <summary>
/// Handler for processing GetSalesQuery requests.
/// </summary>
public class GetSalesHandler : IRequestHandler<GetSalesQuery, GetSalesResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of GetSalesHandler.
    /// </summary>
    /// <param name="saleRepository">The sale repository.</param>
    /// <param name="mapper">The AutoMapper instance.</param>
    public GetSalesHandler(ISaleRepository saleRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the GetSalesQuery request.
    /// </summary>
    /// <param name="query">The GetSales query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paginated sales results with filtering applied.</returns>
    public async Task<GetSalesResult> Handle(GetSalesQuery query, CancellationToken cancellationToken)
    {
        // Build query parameters
        var parameters = new SaleQueryParameters
        {
            Page = query.Page,
            PageSize = query.PageSize,
            OrderBy = query.OrderBy,
            SaleNumber = query.SaleNumber,
            CustomerName = query.CustomerName,
            BranchName = query.BranchName,
            MinSaleDate = query.MinSaleDate,
            MaxSaleDate = query.MaxSaleDate,
            MinTotalAmount = query.MinTotalAmount,
            MaxTotalAmount = query.MaxTotalAmount,
            Status = query.Status
        };

        // Determine which repository method to call based on filters
        PaginatedResult<Domain.Entities.Sale> paginatedSales;

        if (query.CustomerId.HasValue)
        {
            // Filter by customer ID
            paginatedSales = await _saleRepository.GetByCustomerIdAsync(
                query.CustomerId.Value, 
                parameters, 
                cancellationToken);
        }
        else if (query.BranchId.HasValue)
        {
            // Filter by branch ID
            paginatedSales = await _saleRepository.GetByBranchIdAsync(
                query.BranchId.Value, 
                parameters, 
                cancellationToken);
        }
        else if (query.MinSaleDate.HasValue || query.MaxSaleDate.HasValue)
        {
            // Filter by date range
            var startDate = query.MinSaleDate ?? DateTime.MinValue;
            var endDate = query.MaxSaleDate ?? DateTime.MaxValue;
            
            paginatedSales = await _saleRepository.GetByDateRangeAsync(
                startDate, 
                endDate, 
                parameters, 
                cancellationToken);
        }
        else
        {
            // No specific filter - use date range with wide bounds
            // This ensures we can still apply other filters like status, sale number, etc.
            paginatedSales = await _saleRepository.GetByDateRangeAsync(
                DateTime.MinValue, 
                DateTime.MaxValue, 
                parameters, 
                cancellationToken);
        }

        // Map to result
        var result = _mapper.Map<GetSalesResult>(paginatedSales);
        return result;
    }
}
