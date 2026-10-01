namespace AtlasOps.Features.Observability.MetricAlertMonitoring;

public sealed record UpdateMetricAlertMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);