namespace AtlasOps.Features.Cloud.CloudFunctionProvisioning;

public sealed record UpdateCloudFunctionProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);