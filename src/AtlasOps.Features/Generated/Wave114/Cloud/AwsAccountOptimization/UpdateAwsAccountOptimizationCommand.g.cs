namespace AtlasOps.Features.Cloud.AwsAccountOptimization;

public sealed record UpdateAwsAccountOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);