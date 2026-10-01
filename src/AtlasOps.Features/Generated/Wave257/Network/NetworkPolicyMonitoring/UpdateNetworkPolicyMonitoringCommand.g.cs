namespace AtlasOps.Features.Network.NetworkPolicyMonitoring;

public sealed record UpdateNetworkPolicyMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);