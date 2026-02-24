namespace Heartbeat.Producer.Core.Models.Metrics;

public sealed record NetworkMetricsModel(
    int NetworkUsagePercent,
    int NetworkUploadSpeedInMbps,
    int NetworkDownloadSpeedInMbps
);
