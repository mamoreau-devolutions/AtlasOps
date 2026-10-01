namespace AtlasOps.Features.Storage.StorageTransferProvisioning;

public sealed record UpdateStorageTransferProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);