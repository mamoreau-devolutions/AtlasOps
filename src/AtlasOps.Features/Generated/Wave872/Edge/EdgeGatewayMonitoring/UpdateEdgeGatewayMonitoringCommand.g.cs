namespace AtlasOps.Features.Edge.EdgeGatewayMonitoring;

public sealed record UpdateEdgeGatewayMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);