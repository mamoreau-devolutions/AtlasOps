namespace AtlasOps.Features.Cloud.CloudNetworkOptimization;

public sealed record UpdateCloudNetworkOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);