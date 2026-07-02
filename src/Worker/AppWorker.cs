using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using src.Worker.MQTT;

namespace src.Worker;

//TODO: improve class name
public sealed class AppWorker(IMqttBrokerService brokerService, ILogger<AppWorker> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await brokerService.Consume(stoppingToken);
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
            }
        }
    }
}
