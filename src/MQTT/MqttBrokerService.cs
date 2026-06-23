using System.Buffers;
using System.Text;
using Microsoft.Extensions.Hosting;
using src.MQTT.Connection;

namespace src.MQTT;

public sealed class MqttBrokerService(IMqttConnectionManager connectionManager) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var mqttClient = await connectionManager.TryGetConnection(stoppingToken);
                mqttClient.ApplicationMessageReceivedAsync += e =>
                {
                    Console.WriteLine(
                        $"message recived {ParseBytesToUtf8(e.ApplicationMessage.Payload)}"
                    );
                    return Task.CompletedTask;
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }

    private static string ParseBytesToUtf8(ReadOnlySequence<byte> byteSpan) =>
        Encoding.UTF8.GetString(byteSpan);
}
