namespace AtlasOps.Features.Edge.EdgeIncidentMonitoring;

public sealed record UpdateEdgeIncidentMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);