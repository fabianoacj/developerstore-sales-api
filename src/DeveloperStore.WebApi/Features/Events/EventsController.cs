using DeveloperStore.Application.Events.GetAllEvents;
using DeveloperStore.Application.Events.GetSaleEvents;
using DeveloperStore.WebApi.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DeveloperStore.WebApi.Features.Events;

/// <summary>
/// Controller for querying sales events from MongoDB event store.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class EventsController : BaseController
{
    private readonly IMediator _mediator;

    public EventsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves all sales events from the event store.
    /// </summary>
    /// <param name="queryParams">Query parameters for pagination.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of all sales events.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponseWithData<GetAllEventsResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponseWithValidationErrors), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllEvents(
        [FromQuery] EventsQueryParams queryParams,
        CancellationToken cancellationToken = default)
    {
        // Convert page/pageSize to skip/limit for the query
        var skip = (queryParams._page - 1) * queryParams._size;

        var query = new GetAllEventsQuery
        {
            Skip = skip,
            Limit = queryParams._size
        };

        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Retrieves all events for a specific sale.
    /// </summary>
    /// <param name="saleId">The unique identifier of the sale.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Timeline of events for the specified sale.</returns>
    [HttpGet("sales/{saleId}")]
    [ProducesResponseType(typeof(ApiResponseWithData<GetSaleEventsResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponseWithValidationErrors), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSaleEvents(
        [FromRoute] Guid saleId,
        CancellationToken cancellationToken = default)
    {
        var query = new GetSaleEventsQuery(saleId);
        var result = await _mediator.Send(query, cancellationToken);

        if (result.Count == 0)
        {
            return NotFound($"No events found for sale with ID: {saleId}");
        }

        return Ok(result);
    }
}
