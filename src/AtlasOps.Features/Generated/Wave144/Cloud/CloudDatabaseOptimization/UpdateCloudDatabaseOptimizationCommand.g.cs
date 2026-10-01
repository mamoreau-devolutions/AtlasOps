namespace AtlasOps.Features.Cloud.CloudDatabaseOptimization;

public sealed record UpdateCloudDatabaseOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);