namespace AtlasOps.Features.Cloud.AzureSubscriptionProvisioning;

public sealed record UpdateAzureSubscriptionProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);