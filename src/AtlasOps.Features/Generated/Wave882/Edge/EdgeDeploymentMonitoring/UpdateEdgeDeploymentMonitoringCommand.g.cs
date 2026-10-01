namespace AtlasOps.Features.Edge.EdgeDeploymentMonitoring;

public sealed record UpdateEdgeDeploymentMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);