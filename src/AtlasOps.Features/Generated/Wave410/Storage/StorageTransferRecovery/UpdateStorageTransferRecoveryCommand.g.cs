namespace AtlasOps.Features.Storage.StorageTransferRecovery;

public sealed record UpdateStorageTransferRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);