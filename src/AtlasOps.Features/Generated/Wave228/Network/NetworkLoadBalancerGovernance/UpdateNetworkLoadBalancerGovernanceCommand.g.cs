namespace AtlasOps.Features.Network.NetworkLoadBalancerGovernance;

public sealed record UpdateNetworkLoadBalancerGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);