namespace AtlasOps.Features.Data.DataQualityOptimization;

public sealed record UpdateDataQualityOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);