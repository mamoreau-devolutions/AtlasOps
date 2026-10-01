namespace AtlasOps.Features.Data.DataAccessOptimization;

public sealed record UpdateDataAccessOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);