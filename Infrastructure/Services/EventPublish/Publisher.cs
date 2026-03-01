using Confluent.Kafka;

namespace Heartbeat.Producer.Infrastructure.Services.EventPublish;

public class Publisher(PublisherOptions options) : IPublisher
{
    public async Task<bool> PublishEventAsync<T>(T eventData, CancellationToken cancellationToken)
    {
        var message = new Message<string, T> { Key = Guid.NewGuid().ToString(), Value = eventData };
        var config = GetProducerConfig();
        var producer = new ProducerBuilder<string, T>(config).Build();
        var result = await producer.ProduceAsync(options.TopicName, message, cancellationToken);
        return result.Status == PersistenceStatus.Persisted;
    }

    private ProducerConfig GetProducerConfig() =>
        new()
        {
            BootstrapServers = options.BootstrapServe,
            ClientId = options.ClientId,
            SecurityProtocol = GetSecurityProtocol(options.SecurityProtocol),
            Acks = GetAcks(options.Acks),
            MessageTimeoutMs = options.MessageTimeout,
            BatchNumMessages = options.BachNumMessages,
            LingerMs = options.LingerMs,
            CompressionType = GetCompressionType(options.CompressionType),
        };

    private static SecurityProtocol GetSecurityProtocol(string protocol) =>
        protocol switch
        {
            "PLAINTEXT" => SecurityProtocol.Plaintext,
            "SSL" => SecurityProtocol.Ssl,
            "SASL_PLAINTEXT" => SecurityProtocol.SaslPlaintext,
            "SASL_SSL" => SecurityProtocol.SaslSsl,
            _ => SecurityProtocol.Plaintext,
        };

    private static Acks GetAcks(string? acks) =>
        acks switch
        {
            "All" => Acks.All,
            "Leader" => Acks.Leader,
            "None" => Acks.None,
            _ => Acks.All,
        };

    private static CompressionType GetCompressionType(string? compressionType) =>
        compressionType switch
        {
            "None" => CompressionType.None,
            "Gzip" => CompressionType.Gzip,
            "Snappy" => CompressionType.Snappy,
            "Lz4" => CompressionType.Lz4,
            _ => CompressionType.None,
        };
}
