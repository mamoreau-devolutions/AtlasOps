namespace AtlasOps.Features.Storage.StorageReplicationRecovery;

public sealed record UpdateStorageReplicationRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);