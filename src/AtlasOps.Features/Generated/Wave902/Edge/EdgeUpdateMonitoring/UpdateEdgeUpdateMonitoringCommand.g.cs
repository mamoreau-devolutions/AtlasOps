namespace AtlasOps.Features.Edge.EdgeUpdateMonitoring;

public sealed record UpdateEdgeUpdateMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);