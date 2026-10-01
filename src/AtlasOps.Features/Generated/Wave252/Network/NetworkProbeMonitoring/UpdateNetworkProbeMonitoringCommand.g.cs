namespace AtlasOps.Features.Network.NetworkProbeMonitoring;

public sealed record UpdateNetworkProbeMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);