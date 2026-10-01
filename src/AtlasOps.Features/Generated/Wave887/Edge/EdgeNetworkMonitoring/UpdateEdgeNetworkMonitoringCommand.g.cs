namespace AtlasOps.Features.Edge.EdgeNetworkMonitoring;

public sealed record UpdateEdgeNetworkMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);