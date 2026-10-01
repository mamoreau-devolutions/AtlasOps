namespace AtlasOps.Features.Delivery.ReleaseMetricMonitoring;

public sealed record UpdateReleaseMetricMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);