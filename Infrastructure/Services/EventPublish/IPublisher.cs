namespace Heartbeat.Producer.Infrastructure.Services.EventPublish;

public interface IPublisher
{
    Task<bool> PublishEventAsync<T>(T eventData, CancellationToken cancellationToken);
}