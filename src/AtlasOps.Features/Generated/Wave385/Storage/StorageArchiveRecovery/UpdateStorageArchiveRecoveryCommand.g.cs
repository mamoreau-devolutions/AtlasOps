namespace AtlasOps.Features.Storage.StorageArchiveRecovery;

public sealed record UpdateStorageArchiveRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);