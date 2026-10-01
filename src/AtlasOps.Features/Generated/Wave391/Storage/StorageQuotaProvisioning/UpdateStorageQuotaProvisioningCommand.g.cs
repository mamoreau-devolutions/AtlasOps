namespace AtlasOps.Features.Storage.StorageQuotaProvisioning;

public sealed record UpdateStorageQuotaProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);