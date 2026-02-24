using Heartbeat.Producer.Core.Contracts;
using Heartbeat.Producer.Core.Models.Metrics;

namespace Heartbeat.Producer.Infrastructure.Stubs.Contracts;

public class MetricCollectorStub : IMetricCollector
{
    private readonly Random _random = new();
    public string Name { get; }

    // TODO: Create real implementation using System.Management
    public Task<T> CollectMetricsAsync<T>(CancellationToken cancellationToken)
    {
        // TODO: Create a logical to calls to method based on name property
        throw new NotImplementedException();
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
