namespace AtlasOps.Features.Observability.LogSourceOptimization;

public sealed record UpdateLogSourceOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);