namespace AtlasOps.Features.Observability.ObservabilityDashboardOptimization;

public sealed record UpdateObservabilityDashboardOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);