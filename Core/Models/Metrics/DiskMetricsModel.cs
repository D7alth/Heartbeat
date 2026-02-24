namespace Heartbeat.Producer.Core.Models.Metrics;

public sealed record DiskMetricsModel(
    int DiskUsagePercent,
    int DiskUsageInGigaBytes,
    int DiskTotalInGigaBytes,
    int DiskReadSpeedInMBps,
    int DiskWriteSpeedInMBps
);
