namespace AtlasOps.Features.Cloud.CloudBillingOptimization;

public sealed record UpdateCloudBillingOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);