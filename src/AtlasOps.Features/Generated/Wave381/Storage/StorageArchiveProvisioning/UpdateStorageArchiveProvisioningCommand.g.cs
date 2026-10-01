namespace AtlasOps.Features.Storage.StorageArchiveProvisioning;

public sealed record UpdateStorageArchiveProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);