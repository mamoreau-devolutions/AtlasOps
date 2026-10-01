namespace AtlasOps.Features.Network.NetworkPeerMonitoring;

public sealed record UpdateNetworkPeerMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);