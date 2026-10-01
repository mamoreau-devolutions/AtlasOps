namespace AtlasOps.Features.Edge.EdgeNetworkOptimization;

public sealed record UpdateEdgeNetworkOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);