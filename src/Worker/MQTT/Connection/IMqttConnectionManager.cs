using MQTTnet;

namespace src.Worker.MQTT.Connection;

public interface IMqttConnectionManager
{
    Task<IMqttClient?> TryGetConnection(CancellationToken cancellationToken);
}
