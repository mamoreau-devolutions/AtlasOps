namespace AtlasOps.Features.Storage.StorageSnapshotProvisioning;

public sealed record UpdateStorageSnapshotProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);