namespace AtlasOps.Features.Storage.StorageSnapshotOptimization;

public sealed record UpdateStorageSnapshotOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);