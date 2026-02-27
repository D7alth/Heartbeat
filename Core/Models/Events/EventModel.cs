namespace Heartbeat.Producer.Core.Models.Events;

public sealed class EventModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime DateTime { get; set; } = DateTime.UtcNow;
    public string Environment { get; init; } = string.Empty;
    public string ApplicationName { get; init; } = string.Empty;
    public EventData EventData { get; init; } = null!;
}
