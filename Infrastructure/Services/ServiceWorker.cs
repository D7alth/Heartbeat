using Heartbeat.Producer.Core.Contracts;
using Heartbeat.Producer.Core.Helpers.GetMetrics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Heartbeat.Producer.Infrastructure.Services;

internal sealed class ServiceWorker(IMetricCollector metricCollector, ILogger<ServiceWorker> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (true)
        {
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
            await Task.Delay(10_000, stoppingToken);
        }
    }
}
