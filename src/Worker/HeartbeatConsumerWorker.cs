using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet;
using src.Infrastructure.Messaging;
using src.Worker.MQTT.Connection;

namespace src.Worker;

//TODO: improve class name
public sealed class HeartbeatConsumerWorker(
    IMqttConnectionManager connectionManager,
    ILogger<HeartbeatConsumerWorker> logger
) : BackgroundService
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
                await Consume(_mqttClient, stoppingToken);
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
            }
        }
    }

    private static Task Consume(IMqttClient client, CancellationToken cancellationToken)
    {
        client.ApplicationMessageReceivedAsync += async e =>
        {
            var message = MessageFactory.CreateMessageFromMqtt(e);
            //TODO: Add something like Dispatcher or mediator pipeline to call our Features/useCase layer
            await e.AcknowledgeAsync(cancellationToken);
        };
        return Task.CompletedTask;
    }
}
