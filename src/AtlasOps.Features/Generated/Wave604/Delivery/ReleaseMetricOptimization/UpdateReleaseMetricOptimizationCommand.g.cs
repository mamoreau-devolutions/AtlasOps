namespace AtlasOps.Features.Delivery.ReleaseMetricOptimization;

public sealed record UpdateReleaseMetricOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);