namespace AtlasOps.Features.Data.DataLineageOptimization;

public sealed record UpdateDataLineageOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);