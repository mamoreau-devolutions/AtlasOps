namespace AtlasOps.Features.Edge.EdgeApplicationOptimization;

public sealed record UpdateEdgeApplicationOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);