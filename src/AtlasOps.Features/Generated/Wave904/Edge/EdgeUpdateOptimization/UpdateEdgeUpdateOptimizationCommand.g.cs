namespace AtlasOps.Features.Edge.EdgeUpdateOptimization;

public sealed record UpdateEdgeUpdateOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);