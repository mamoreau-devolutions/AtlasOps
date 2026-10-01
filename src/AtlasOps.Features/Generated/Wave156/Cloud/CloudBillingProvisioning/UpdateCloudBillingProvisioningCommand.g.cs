namespace AtlasOps.Features.Cloud.CloudBillingProvisioning;

public sealed record UpdateCloudBillingProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);