using AutoMapper;
using DeveloperStore.Application.Sales.CancelSale;
using DeveloperStore.Application.Sales.CancelSaleItem;
using DeveloperStore.Application.Sales.CreateSale;
using DeveloperStore.Application.Sales.GetSale;
using DeveloperStore.Application.Sales.GetSales;
using DeveloperStore.Application.Sales.UpdateSale;
using DeveloperStore.WebApi.Common;
using DeveloperStore.WebApi.Features.Sales.CreateSale;
using DeveloperStore.WebApi.Features.Sales.GetSale;
using DeveloperStore.WebApi.Features.Sales.UpdateSale;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DeveloperStore.WebApi.Features.Sales;

/// <summary>
/// Controller for managing sales operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SalesController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public SalesController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    /// <summary>
    /// Creates a new sale.
    /// </summary>
    /// <param name="request">The sale creation request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created sale details.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponseWithData<CreateSaleResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponseWithValidationErrors), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateSale([FromBody] CreateSaleRequest request, CancellationToken cancellationToken)
    {
        var command = _mapper.Map<CreateSaleCommand>(request);
        var result = await _mediator.Send(command, cancellationToken);
        var response = _mapper.Map<CreateSaleResponse>(result);

        return Created("GetSale", new { id = response.Id }, response);
    }

    /// <summary>
    /// Retrieves a sale by its ID.
    /// </summary>
    /// <param name="id">The unique identifier of the sale.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The sale details if found.</returns>
    [HttpGet("{id}", Name = "GetSale")]
    [ProducesResponseType(typeof(ApiResponseWithData<GetSaleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponseWithValidationErrors), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSale([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var command = new GetSaleCommand(id);
        var result = await _mediator.Send(command, cancellationToken);
        var response = _mapper.Map<GetSaleResponse>(result);

        return Ok(response);
    }

    /// <summary>
    /// Gets all sales with flexible filtering, pagination, and sorting.
    /// </summary>
    /// <param name="queryParams">Query parameters for filtering, pagination, and sorting.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paginated list of sales matching the criteria.</returns>
    /// <remarks>
    /// This consolidated endpoint supports filtering by:
    /// - Customer ID (customerId)
    /// - Branch ID (branchId)
    /// - Sale number (saleNumber) - supports wildcards with *
    /// - Customer name (customerName) - supports wildcards with *
    /// - Branch name (branchName) - supports wildcards with *
    /// - Date range (_minSaleDate, _maxSaleDate)
    /// - Amount range (_minTotalAmount, _maxTotalAmount)
    /// - Status (status: Active, Cancelled)
    /// 
    /// Examples:
    /// - All sales: GET /api/sales
    /// - By customer: GET /api/sales?customerId=123e4567-e89b-12d3-a456-426614174000
    /// - By branch: GET /api/sales?branchId=123e4567-e89b-12d3-a456-426614174001
    /// - Active sales only: GET /api/sales?status=Active
    /// - Combined filters: GET /api/sales?customerId=123&amp;status=Active&amp;_minTotalAmount=100
    /// </remarks>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponseWithData<GetSalesResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponseWithValidationErrors), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetSales(
        [FromQuery] SalesQueryParams queryParams,
        CancellationToken cancellationToken)
    {
        var query = new GetSalesQuery
        {
            Page = queryParams._page,
            PageSize = queryParams._size,
            OrderBy = queryParams._order,
            CustomerId = queryParams.customerId,
            BranchId = queryParams.branchId,
            SaleNumber = queryParams.saleNumber,
            CustomerName = queryParams.customerName,
            BranchName = queryParams.branchName,
            MinSaleDate = queryParams._minSaleDate,
            MaxSaleDate = queryParams._maxSaleDate,
            MinTotalAmount = queryParams._minTotalAmount,
            MaxTotalAmount = queryParams._maxTotalAmount,
            Status = string.IsNullOrEmpty(queryParams.status) ? null :
                Enum.TryParse<Domain.Enums.SaleStatus>(queryParams.status, true, out var status) ? status : null
        };

        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Updates an existing sale.
    /// </summary>
    /// <param name="id">The sale ID.</param>
    /// <param name="request">The update request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated sale details.</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponseWithData<UpdateSaleResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponseWithValidationErrors), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateSale([FromRoute] Guid id, [FromBody] UpdateSaleRequest request, CancellationToken cancellationToken)
    {
        var command = _mapper.Map<UpdateSaleCommand>(request);
        command.Id = id;

        var result = await _mediator.Send(command, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Cancels a sale.
    /// </summary>
    /// <param name="id">The sale ID to cancel.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Cancellation confirmation.</returns>
    [HttpPost("{id}/cancel")]
    [ProducesResponseType(typeof(ApiResponseWithData<CancelSaleResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelSale([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var command = new CancelSaleCommand(id);
        var result = await _mediator.Send(command, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Cancels a sale item.
    /// </summary>
    /// <param name="id">The sale ID.</param>
    /// <param name="itemId">The sale item ID to cancel.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Cancellation confirmation.</returns>
    [HttpPost("{id}/items/{itemId}/cancel")]
    [ProducesResponseType(typeof(ApiResponseWithData<CancelSaleItemResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelSaleItem([FromRoute] Guid id, [FromRoute] Guid itemId, CancellationToken cancellationToken)
    {
        var command = new CancelSaleItemCommand(id, itemId);
        var result = await _mediator.Send(command, cancellationToken);

        return Ok(result);
    }
}


