namespace AtlasOps.Features.Storage.StorageEncryptionProvisioning;

public sealed record UpdateStorageEncryptionProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);