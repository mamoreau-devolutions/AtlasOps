namespace AtlasOps.Features.Cloud.CloudRegionOptimization;

public sealed record UpdateCloudRegionOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);