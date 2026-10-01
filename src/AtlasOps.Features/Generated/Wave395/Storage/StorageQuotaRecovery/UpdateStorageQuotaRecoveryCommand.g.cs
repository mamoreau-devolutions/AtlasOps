namespace AtlasOps.Features.Storage.StorageQuotaRecovery;

public sealed record UpdateStorageQuotaRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);