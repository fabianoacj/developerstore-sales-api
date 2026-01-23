namespace DeveloperStore.WebApi.Features.Events;

/// <summary>
/// Query parameters for events pagination.
/// </summary>
public class EventsQueryParams
{
    /// <summary>
    /// Page number (default: 1).
    /// </summary>
    public int _page { get; set; } = 1;

    /// <summary>
    /// Number of items per page (default: 100, max: 500).
    /// </summary>
    public int _size { get; set; } = 100;
}
