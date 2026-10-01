namespace AtlasOps.Features.Observability.LogQueryOptimization;

public sealed record UpdateLogQueryOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);