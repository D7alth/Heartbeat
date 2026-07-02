using MQTTnet;
using src.Worker.MQTT.Configuration;

namespace src.Worker.MQTT.Connection;

public static class MqttOptionsBuilder
{
    public static MqttClientOptions Build(MqttBrokerOptions options) =>
        new MqttClientOptionsBuilder()
            .WithTcpServer(options.Broker, options.Port)
            .WithCredentials(options.Username, options.Password)
            .WithClientId(options.ClientId)
            .Build();
}
