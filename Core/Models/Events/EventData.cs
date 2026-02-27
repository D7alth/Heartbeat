using Heartbeat.Producer.Core.Models.Metrics;

namespace Heartbeat.Producer.Core.Models.Events;

public sealed record EventData(
    CpuMetricsModel CpuMetrics,
    DiskMetricsModel DiskMetrics,
    NetworkMetricsModel NetworkMetrics
);
