using Confluent.Kafka;
using Heartbeat.Producer.Core.Models.Messaging;

namespace Heartbeat.Producer.Infrastructure.Messaging.Connection;

public sealed class KafkaConnectionManager : IKafkaConnectionManager, IDisposable
{
    private volatile IProducer<string, byte[]>? _producer;
    private volatile ConnectionState _state = ConnectionState.Starting;
    public ConnectionState State => _state;

    public IProducer<string, byte[]>? GetProducer() =>
        _state == ConnectionState.Ready ? _producer : null;

    public void SetProducer(IProducer<string, byte[]> producer)
    {
        var previous = Interlocked.Exchange(ref _producer, producer);
        previous?.Dispose();
        _state = ConnectionState.Ready;
    }

    public void MarkDegraded() => _state = ConnectionState.Degraded;

    public void Dispose() => _producer?.Dispose();
}
