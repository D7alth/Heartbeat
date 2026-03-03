using Heartbeat.Producer.Core.Contracts;
using Heartbeat.Producer.Core.Models.Events;
using Heartbeat.Producer.Core.Models.Metrics;

namespace Heartbeat.Producer.Core.Helpers.GetMetrics;

public static class GetMetricsHelper
{
    public static async Task<EventModel> GetMetricsAsync(
        IMetricCollector metricCollector,
        CancellationToken cancellationToken
    )
    {
        var cpuMetricsTask = GetCpuMetricsAsync(metricCollector, cancellationToken);
        var diskMetricsTask = GetDiskMetricsAsync(metricCollector, cancellationToken);
        var networkMetricsTask = GetNetworkMetricsAsync(metricCollector, cancellationToken);
        await Task.WhenAll(cpuMetricsTask, diskMetricsTask, networkMetricsTask);
        var eventData = new EventData(
            cpuMetricsTask.Result,
            diskMetricsTask.Result,
            networkMetricsTask.Result
        );
        var eventModel = new EventModel
        {
            Environment = "Application",
            ApplicationName = "Heartbeat.Producer",
            EventData = eventData,
        };
        return eventModel;
    }

    private static async Task<CpuMetricsModel> GetCpuMetricsAsync(
        IMetricCollector metricCollector,
        CancellationToken cancellationToken
    ) =>
        await metricCollector.CollectMetricsAsync<CpuMetricsModel>(
            nameof(CpuMetricsModel),
            cancellationToken
        );

    private static async Task<DiskMetricsModel> GetDiskMetricsAsync(
        IMetricCollector metricCollector,
        CancellationToken cancellationToken
    ) =>
        await metricCollector.CollectMetricsAsync<DiskMetricsModel>(
            nameof(DiskMetricsModel),
            cancellationToken
        );

    private static async Task<NetworkMetricsModel> GetNetworkMetricsAsync(
        IMetricCollector metricCollector,
        CancellationToken cancellationToken
    ) =>
        await metricCollector.CollectMetricsAsync<NetworkMetricsModel>(
            nameof(NetworkMetricsModel),
            cancellationToken
        );
}
