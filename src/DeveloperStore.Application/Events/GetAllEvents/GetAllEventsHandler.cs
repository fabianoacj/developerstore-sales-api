using DeveloperStore.Domain.Repositories;
using MediatR;

namespace DeveloperStore.Application.Events.GetAllEvents;

/// <summary>
/// Handler for retrieving all events from the event store.
/// </summary>
public class GetAllEventsHandler : IRequestHandler<GetAllEventsQuery, GetAllEventsResult>
{
    private readonly IEventStore _eventStore;

    public GetAllEventsHandler(IEventStore eventStore)
    {
        _eventStore = eventStore;
    }

    public async Task<GetAllEventsResult> Handle(GetAllEventsQuery request, CancellationToken cancellationToken)
    {
        var events = await _eventStore.GetAllEventsAsync(
            request.Skip, 
            request.Limit, 
            cancellationToken);

        var eventDtos = events.Select(e => new EventDto
        {
            Id = e.Id,
            EventType = e.EventType,
            EventData = e.EventData,
            OccurredAt = e.OccurredAt,
            SaleId = e.AggregateId,
            SaleNumber = e.SaleNumber
        }).ToList();

        return new GetAllEventsResult
        {
            Events = eventDtos
        };
    }
}
