namespace AtlasOps.Features.Cloud.CloudIdentityProvisioning;

public sealed record UpdateCloudIdentityProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);