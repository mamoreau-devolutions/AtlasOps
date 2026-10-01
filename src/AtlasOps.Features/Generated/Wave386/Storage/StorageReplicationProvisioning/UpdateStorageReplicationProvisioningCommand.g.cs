namespace AtlasOps.Features.Storage.StorageReplicationProvisioning;

public sealed record UpdateStorageReplicationProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);