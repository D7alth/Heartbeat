using Heartbeat.Producer.Core.Contracts;
using Heartbeat.Producer.Core.Models.Metrics;

namespace Heartbeat.Producer.Infrastructure.Stubs.Contracts;

public class MetricCollectorStub : IMetricCollector
{
    private readonly Random _random = new();

    // TODO: Create real implementation using System.Management
    public async Task<T> CollectMetricsAsync<T>(
        string metricType,
        CancellationToken cancellationToken = default
    )
    {
        var metricTask = metricType switch
        {
            nameof(CpuMetricsModel) => CpuCollectorAsync(cancellationToken) as Task<T>,
            nameof(DiskMetricsModel) => DiskCollectorAsync(cancellationToken) as Task<T>,
            nameof(NetworkMetricsModel) => MemoryCollectorAsync(cancellationToken) as Task<T>,
            _ => null,
        };
        if (metricTask is null)
            return default!;
        return await metricTask;
    }

    private async Task<CpuMetricsModel> CpuCollectorAsync(
        CancellationToken cancellationToken = default
    ) => await Task.FromResult(new CpuMetricsModel(_random.Next(1, 100)));

    private async Task<DiskMetricsModel> DiskCollectorAsync(
        CancellationToken cancellationToken = default
    ) =>
        await Task.FromResult(
            new DiskMetricsModel(
                _random.Next(1, 100),
                _random.Next(1, 1000),
                _random.Next(1, 1000),
                _random.Next(1, 10000),
                _random.Next(1, 1000)
            )
        );

    private async Task<NetworkMetricsModel> MemoryCollectorAsync(
        CancellationToken cancellationToken = default
    ) =>
        await Task.FromResult(
            new NetworkMetricsModel(
                _random.Next(1, 100),
                _random.Next(1, 1000),
                _random.Next(1, 1000)
            )
        );
}
