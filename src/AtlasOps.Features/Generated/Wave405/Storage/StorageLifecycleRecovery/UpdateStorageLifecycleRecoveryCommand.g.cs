namespace AtlasOps.Features.Storage.StorageLifecycleRecovery;

public sealed record UpdateStorageLifecycleRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);