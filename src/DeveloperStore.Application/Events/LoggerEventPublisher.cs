using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace DeveloperStore.Application.Events;

/// <summary>
/// Event publisher that logs events instead of publishing to a message broker.
/// </summary>
public class LoggerEventPublisher : IEventPublisher
{
    private readonly ILogger<LoggerEventPublisher> _logger;

    public LoggerEventPublisher(ILogger<LoggerEventPublisher> logger)
    {
        _logger = logger;
    }

    public Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : class
    {
        var eventType = typeof(TEvent).Name;
        var eventData = JsonSerializer.Serialize(@event, new JsonSerializerOptions { WriteIndented = false });

        _logger.LogInformation(
            "Event Published: {EventType} | Data: {EventData}",
            eventType,
            eventData);

        return Task.CompletedTask;
    }
}
