namespace Heartbeat.Producer.Core.Contracts.Messaging;

public interface IPublisher
{
    Task<bool> PublishEventAsync(
        string topicName,
        byte[] eventData,
        CancellationToken cancellationToken
    );
}
