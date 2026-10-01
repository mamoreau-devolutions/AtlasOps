namespace AtlasOps.Features.Data.DataTransformOptimization;

public sealed record UpdateDataTransformOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);