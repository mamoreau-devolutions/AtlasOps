namespace AtlasOps.Features.Network.NetworkSegmentMonitoring;

public sealed record UpdateNetworkSegmentMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);