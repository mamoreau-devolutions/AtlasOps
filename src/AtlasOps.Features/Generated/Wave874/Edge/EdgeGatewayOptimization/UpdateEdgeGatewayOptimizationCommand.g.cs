namespace AtlasOps.Features.Edge.EdgeGatewayOptimization;

public sealed record UpdateEdgeGatewayOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);