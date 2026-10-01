namespace AtlasOps.Features.Cloud.AzureSubscriptionRecovery;

public sealed record UpdateAzureSubscriptionRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);