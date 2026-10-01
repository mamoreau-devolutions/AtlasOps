namespace AtlasOps.Features.Storage.StorageEncryptionRecovery;

public sealed record UpdateStorageEncryptionRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);