using DeveloperStore.Application.Events.GetAllEvents;
using DeveloperStore.Domain.Repositories;
using DeveloperStore.Domain.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DeveloperStore.Application.Events.GetSaleEvents;

/// <summary>
/// Handler for retrieving events for a specific sale.
/// Implements caching for improved performance since events are immutable.
/// </summary>
public class GetSaleEventsHandler : IRequestHandler<GetSaleEventsQuery, GetSaleEventsResult>
{
    private readonly IEventStore _eventStore;
    private readonly ICacheService _cacheService;
    private readonly ILogger<GetSaleEventsHandler> _logger;
    private const string CacheKeyPrefix = "sale-events";
    private static readonly TimeSpan CacheExpiration = TimeSpan.FromHours(1);

    public GetSaleEventsHandler(
        IEventStore eventStore, 
        ICacheService cacheService,
        ILogger<GetSaleEventsHandler> logger)
    {
        _eventStore = eventStore;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<GetSaleEventsResult> Handle(GetSaleEventsQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"{CacheKeyPrefix}:{request.SaleId}";

        // Try to get from cache first
        var cachedResult = await _cacheService.GetAsync<GetSaleEventsResult>(cacheKey, cancellationToken);
        if (cachedResult != null)
        {
            _logger.LogInformation("Retrieved sale events from cache for SaleId: {SaleId}", request.SaleId);
            return cachedResult;
        }

        _logger.LogInformation("Cache miss for sale events. Querying event store for SaleId: {SaleId}", request.SaleId);

        // Cache miss - query the event store
        var events = await _eventStore.GetEventsBySaleIdAsync(request.SaleId, cancellationToken);

        var eventDtos = events.Select(e => new EventDto
        {
            Id = e.Id,
            EventType = e.EventType,
            EventData = e.EventData,
            OccurredAt = e.OccurredAt,
            SaleId = e.AggregateId,
            SaleNumber = e.SaleNumber
        }).ToList();

        var result = new GetSaleEventsResult
        {
            SaleId = request.SaleId,
            Events = eventDtos
        };

        // Cache the result
        // Note: Events are immutable (append-only), so we can cache indefinitely
        // Using 1 hour expiration as a safety measure
        await _cacheService.SetAsync(cacheKey, result, CacheExpiration, cancellationToken);
        _logger.LogInformation("Cached sale events for SaleId: {SaleId}", request.SaleId);

        return result;
    }
}
