using Heartbeat.Producer.Core.Contracts;
using Heartbeat.Producer.Core.Contracts.Messaging;
using Heartbeat.Producer.Core.Helpers.GetMetrics;
using Heartbeat.Producer.Core.Models.Events;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Heartbeat.Producer.Infrastructure.Services;

internal sealed class ServiceWorker(
    IMetricCollector metricCollector,
    IPublisher publish,
    ILogger<ServiceWorker> logger
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (true)
        {
            await Task.Delay(1_000, stoppingToken);
            var eventModel = await GetMetricsHelper.GetMetricsAsync(metricCollector, stoppingToken);
            logger.LogInformation(
                @$"Id: {eventModel.Id} 
                Environment: {eventModel.Environment} 
                Application Name: {eventModel.ApplicationName} 
                DateTime: {eventModel.DateTime}
                Cpu Metrics: {eventModel.EventData.CpuMetrics}
                Disk Metrics: {eventModel.EventData.DiskMetrics}
                Network Metrics: {eventModel.EventData.NetworkMetrics}"
            );
            try
            {
                var eventData = SerializeEvent(eventModel);
                var result = await publish.PublishEventAsync("teste", eventData, stoppingToken);
                if (result)
                {
                    logger.LogInformation(
                        $"Event with Id: {eventModel.Id} published successfully."
                    );
                }
                else
                {
                    logger.LogError($"Failed to publish event with Id: {eventModel.Id}.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    $"An error occurred while publishing event with Id: {eventModel.Id}."
                );
                throw;
            }
        }
    }

    private static byte[] SerializeEvent(EventModel eventModel) =>
        System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(eventModel);
}
