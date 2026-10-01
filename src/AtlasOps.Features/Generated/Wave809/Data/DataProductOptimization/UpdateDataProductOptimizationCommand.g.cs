namespace AtlasOps.Features.Data.DataProductOptimization;

public sealed record UpdateDataProductOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);