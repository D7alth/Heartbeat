namespace Heartbeat.Producer.Infrastructure.Services.EventPublish;

public sealed class PublisherOptions
{
    public string BootstrapServer { get; set; } = string.Empty;
    public string TopicName { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string SecurityProtocol { get; set; } = "PLAINTEXT";
    public string? Acks { get; set; } = "All";
    public int MessageTimeout { get; set; } = 30000;
    public int BachNumMessages { get; set; } = 1000;
    public int LingerMs { get; set; } = 0;
    public string? CompressionType { get; set; } = "None";
}
