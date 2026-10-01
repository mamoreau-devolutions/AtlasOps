namespace AtlasOps.Features.Cloud.CloudFunctionOptimization;

public sealed record UpdateCloudFunctionOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);