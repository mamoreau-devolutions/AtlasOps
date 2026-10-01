namespace AtlasOps.Features.Storage.ObjectBucketProvisioning;

public sealed record UpdateObjectBucketProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);