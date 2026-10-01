namespace AtlasOps.Features.Network.NetworkAddressMonitoring;

public sealed record UpdateNetworkAddressMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);