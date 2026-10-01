namespace AtlasOps.Features.Edge.EdgeTelemetryOptimization;

public sealed record UpdateEdgeTelemetryOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);