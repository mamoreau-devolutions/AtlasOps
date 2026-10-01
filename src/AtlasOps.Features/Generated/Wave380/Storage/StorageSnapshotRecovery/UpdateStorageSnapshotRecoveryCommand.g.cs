namespace AtlasOps.Features.Storage.StorageSnapshotRecovery;

public sealed record UpdateStorageSnapshotRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);