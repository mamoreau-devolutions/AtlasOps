namespace AtlasOps.Features.Network.NetworkFirewallMonitoring;

public sealed record UpdateNetworkFirewallMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);