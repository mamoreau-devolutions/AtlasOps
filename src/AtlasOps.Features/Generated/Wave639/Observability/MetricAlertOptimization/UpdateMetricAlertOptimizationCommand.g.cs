namespace AtlasOps.Features.Observability.MetricAlertOptimization;

public sealed record UpdateMetricAlertOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);