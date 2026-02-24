namespace Heartbeat.Producer.Core.Contracts;

public interface IMetricCollector
{
    public string Name { get; }
    public Task<T> CollectMetricsAsync<T>(CancellationToken cancellationToken);
}
