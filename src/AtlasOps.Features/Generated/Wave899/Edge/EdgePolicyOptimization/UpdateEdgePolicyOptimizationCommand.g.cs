namespace AtlasOps.Features.Edge.EdgePolicyOptimization;

public sealed record UpdateEdgePolicyOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);