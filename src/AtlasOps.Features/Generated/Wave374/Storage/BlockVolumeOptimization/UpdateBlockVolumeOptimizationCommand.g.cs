namespace AtlasOps.Features.Storage.BlockVolumeOptimization;

public sealed record UpdateBlockVolumeOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);