namespace AtlasOps.Features.Network.NetworkLoadBalancerRecovery;

public sealed record UpdateNetworkLoadBalancerRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);