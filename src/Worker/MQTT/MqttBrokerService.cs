using MQTTnet;
using src.Infrastructure.Messaging;
using src.Worker.MQTT.Connection;

namespace src.Worker.MQTT;

public class MqttBrokerService(IMqttConnectionManager connectionManager) : IMqttBrokerService
{
    private static IMqttClient? _mqttClient;

    public async Task<Message> Consume(CancellationToken cancellationToken)
    {
        _mqttClient ??= await connectionManager.TryGetConnection(cancellationToken);
        if (_mqttClient is null)
            return null!;
        Message message = null!;
        _mqttClient.ApplicationMessageReceivedAsync += async e =>
        {
            message = MessageFactory.CreateMessageFromMqtt(e);
            await e.AcknowledgeAsync(cancellationToken);
        };
        return message;
    }
}
