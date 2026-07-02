using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;
using src.Worker.MQTT.Configuration;

namespace src.Worker.MQTT.Connection;

public sealed class MqttConnection(
    IOptions<MqttBrokerOptions> options,
    ILogger<MqttConnection> logger
) : IMqttConnectionManager
{
    private readonly MqttBrokerOptions _valueOptions = options.Value;

    public async Task<IMqttClient?> TryGetConnection(CancellationToken cancellationToken)
    {
        //TODO: Decompose this class into a some other classes, I must follow SPR.
        try
        {
            var factory = new MqttClientFactory();
            var mqttClient = factory.CreateMqttClient();
            var options = GetBuiltOptions();
            var connection = await mqttClient.ConnectAsync(options, cancellationToken);
            if (connection.ResultCode != MqttClientConnectResultCode.Success)
                return null;
            await mqttClient.SubscribeAsync(
                topic: _valueOptions.Topic, // TODO: Create a specific method to subscribe client to a topic
                cancellationToken: cancellationToken
            );
            return mqttClient;
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            throw;
        }
    }

    private MqttClientOptions GetBuiltOptions() =>
        new MqttClientOptionsBuilder()
            .WithTcpServer(_valueOptions.Broker, _valueOptions.Port)
            .WithCredentials(_valueOptions.Username, _valueOptions.Password)
            .WithClientId(_valueOptions.ClientId)
            .Build();
}
