namespace src.Worker.MQTT.Configuration;

public sealed class MqttBrokerOptions
{
    public required string Broker { get; init; }
    public required string Topic { get; init; }
    public required string Username { get; init; }
    public required string Password { get; init; }
    public int Port { get; init; }
    public string ClientId { get; init; } = Guid.CreateVersion7().ToString();
    public required string TslCertificatePath { get; init; }
}
