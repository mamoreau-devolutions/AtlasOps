namespace AtlasOps.Features.Edge.EdgeSiteMonitoring;

public sealed record UpdateEdgeSiteMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);