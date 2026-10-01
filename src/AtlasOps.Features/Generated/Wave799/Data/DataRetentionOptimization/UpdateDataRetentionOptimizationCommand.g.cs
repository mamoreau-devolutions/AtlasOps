namespace AtlasOps.Features.Data.DataRetentionOptimization;

public sealed record UpdateDataRetentionOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);