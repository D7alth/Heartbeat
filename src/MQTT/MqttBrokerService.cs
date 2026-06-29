using System.Buffers;
using System.Text;
using Microsoft.Extensions.Hosting;
using MQTTnet;
using src.MQTT.Connection;

namespace src.MQTT;

public sealed class MqttBrokerService(IMqttConnectionManager connectionManager) : BackgroundService
{
    private static IMqttClient? _mqttClient;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _mqttClient ??= await connectionManager.TryGetConnection(stoppingToken);
                if (_mqttClient is null)
                    return;
                _mqttClient.ApplicationMessageReceivedAsync += async e =>
                {
                    var message = MessageFactory.CreateMessageFromMqtt(e);
                    await e.AcknowledgeAsync(stoppingToken);
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
