namespace AtlasOps.Features.Cloud.CloudStorageOptimization;

public sealed record UpdateCloudStorageOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);