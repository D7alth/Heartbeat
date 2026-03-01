namespace Heartbeat.Producer.Infrastructure.Services.EventPublish;

public sealed record PublisherOptions(
    string TopicName,
    string BootstrapServe,
    string ClientId,
    string SecurityProtocol,
    string? Acks,
    int MessageTimeout,
    int BachNumMessages,
    int LingerMs,
    string? CompressionType
);
