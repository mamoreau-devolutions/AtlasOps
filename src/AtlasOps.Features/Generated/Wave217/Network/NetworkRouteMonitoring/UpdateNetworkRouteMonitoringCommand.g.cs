namespace AtlasOps.Features.Network.NetworkRouteMonitoring;

public sealed record UpdateNetworkRouteMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);