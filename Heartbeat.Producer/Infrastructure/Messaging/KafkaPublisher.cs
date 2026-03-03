using Confluent.Kafka;
using Heartbeat.Producer.Core.Contracts.Messaging;
using Heartbeat.Producer.Core.Models.Messaging;
using Heartbeat.Producer.Infrastructure.Messaging.Connection;

namespace Heartbeat.Producer.Infrastructure.Messaging;

public class KafkaPublisher(IKafkaConnectionManager connectionManager) : IPublisher
{
    public async Task<bool> PublishEventAsync(
        string topicName,
        byte[] eventData,
        CancellationToken cancellationToken
    )
    {
        var message = new Message<string, byte[]>
        {
            Key = Guid.NewGuid().ToString(),
            Value = eventData,
        };
        var producer = connectionManager.GetProducer();
        if (connectionManager.State != ConnectionState.Ready || producer == null)
            return false;
        var result = await producer.ProduceAsync(topicName, message, cancellationToken);
        return result.Status == PersistenceStatus.Persisted;
    }
}
