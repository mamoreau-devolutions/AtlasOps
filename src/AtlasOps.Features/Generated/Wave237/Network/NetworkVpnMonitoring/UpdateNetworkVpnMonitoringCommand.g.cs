namespace AtlasOps.Features.Network.NetworkVpnMonitoring;

public sealed record UpdateNetworkVpnMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);