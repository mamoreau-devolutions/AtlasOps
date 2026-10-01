namespace AtlasOps.Features.Cloud.CloudNetworkProvisioning;

public sealed record UpdateCloudNetworkProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);