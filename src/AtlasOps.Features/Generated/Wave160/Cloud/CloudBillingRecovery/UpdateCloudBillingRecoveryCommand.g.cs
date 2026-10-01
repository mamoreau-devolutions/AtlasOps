namespace AtlasOps.Features.Cloud.CloudBillingRecovery;

public sealed record UpdateCloudBillingRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);