namespace AtlasOps.Features.Network.NetworkDnsZoneMonitoring;

public sealed record UpdateNetworkDnsZoneMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);