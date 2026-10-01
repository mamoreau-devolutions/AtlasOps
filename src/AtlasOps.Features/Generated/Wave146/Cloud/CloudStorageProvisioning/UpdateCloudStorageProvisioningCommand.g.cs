namespace AtlasOps.Features.Cloud.CloudStorageProvisioning;

public sealed record UpdateCloudStorageProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);