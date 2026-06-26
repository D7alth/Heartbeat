using MQTTnet;

namespace src.MQTT.Connection;

public interface IMqttConnectionManager
{
    Task<IMqttClient?> TryGetConnection(CancellationToken cancellationToken);
}
