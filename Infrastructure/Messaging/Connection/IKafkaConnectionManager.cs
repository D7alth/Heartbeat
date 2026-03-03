using Confluent.Kafka;
using Heartbeat.Producer.Core.Models.Messaging;

namespace Heartbeat.Producer.Infrastructure.Messaging.Connection;

public interface IKafkaConnectionManager
{
    ConnectionState State { get; }

    IProducer<string, byte[]>? GetProducer();

    void SetProducer(IProducer<string, byte[]> producer);

    void MarkDegraded();
}
