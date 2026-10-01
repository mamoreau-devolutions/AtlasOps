namespace AtlasOps.Features.Edge.EdgeDeviceOptimization;

public sealed record UpdateEdgeDeviceOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);