namespace AtlasOps.Features.Cloud.CloudIdentityOptimization;

public sealed record UpdateCloudIdentityOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);