namespace AtlasOps.Features.Api.ApiDeploymentOptimization;

public sealed record UpdateApiDeploymentOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);