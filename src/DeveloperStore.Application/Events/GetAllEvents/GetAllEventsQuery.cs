using MediatR;

namespace DeveloperStore.Application.Events.GetAllEvents;

/// <summary>
/// Query to retrieve all events from the event store.
/// </summary>
public class GetAllEventsQuery : IRequest<GetAllEventsResult>
{
    /// <summary>
    /// Number of events to skip for pagination.
    /// </summary>
    public int Skip { get; set; } = 0;

    /// <summary>
    /// Maximum number of events to return.
    /// </summary>
    public int Limit { get; set; } = 100;
}
