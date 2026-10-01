namespace AtlasOps.Features.Observability.MetricSourceMonitoring;

public sealed record UpdateMetricSourceMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);