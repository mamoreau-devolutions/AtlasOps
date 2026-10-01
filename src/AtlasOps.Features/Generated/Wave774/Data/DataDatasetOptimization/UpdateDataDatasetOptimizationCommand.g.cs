namespace AtlasOps.Features.Data.DataDatasetOptimization;

public sealed record UpdateDataDatasetOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);