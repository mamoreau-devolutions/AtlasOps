namespace AtlasOps.Features.Cloud.AwsAccountProvisioning;

public sealed record UpdateAwsAccountProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);