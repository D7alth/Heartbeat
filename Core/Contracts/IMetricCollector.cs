namespace Heartbeat.Producer.Core.Contracts;

public interface IMetricCollector
{
    public Task<T> CollectMetricsAsync<T>(string metricType, CancellationToken cancellationToken);
}
