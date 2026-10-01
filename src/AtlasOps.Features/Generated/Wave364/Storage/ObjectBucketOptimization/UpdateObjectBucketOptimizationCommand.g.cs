namespace AtlasOps.Features.Storage.ObjectBucketOptimization;

public sealed record UpdateObjectBucketOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);