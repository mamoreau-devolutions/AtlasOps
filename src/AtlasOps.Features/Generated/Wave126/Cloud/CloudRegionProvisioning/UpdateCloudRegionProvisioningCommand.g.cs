namespace AtlasOps.Features.Cloud.CloudRegionProvisioning;

public sealed record UpdateCloudRegionProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);