namespace AtlasOps.Features.Storage.StorageReplicationOptimization;

public sealed record UpdateStorageReplicationOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);