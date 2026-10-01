namespace AtlasOps.Features.Edge.EdgeDeviceMonitoring;

public sealed record UpdateEdgeDeviceMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);