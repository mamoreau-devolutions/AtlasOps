namespace AtlasOps.Features.Edge.EdgeApplicationMonitoring;

public sealed record UpdateEdgeApplicationMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);