namespace AtlasOps.Features.Storage.StorageLifecycleOptimization;

public sealed record UpdateStorageLifecycleOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);