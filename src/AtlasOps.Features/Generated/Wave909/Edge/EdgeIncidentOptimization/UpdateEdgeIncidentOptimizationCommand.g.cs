namespace AtlasOps.Features.Edge.EdgeIncidentOptimization;

public sealed record UpdateEdgeIncidentOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);