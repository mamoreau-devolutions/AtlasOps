namespace AtlasOps.Features.Network.NetworkLoadBalancerMonitoring;

public sealed record UpdateNetworkLoadBalancerMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);