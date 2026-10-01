namespace AtlasOps.Features.Cloud.AzureSubscriptionOptimization;

public sealed record UpdateAzureSubscriptionOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);