namespace AtlasOps.Features.Edge.EdgeDeploymentOptimization;

public sealed record UpdateEdgeDeploymentOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);