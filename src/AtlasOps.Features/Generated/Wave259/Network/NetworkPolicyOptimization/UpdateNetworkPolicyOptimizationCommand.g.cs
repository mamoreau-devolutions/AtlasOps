namespace AtlasOps.Features.Network.NetworkPolicyOptimization;

public sealed record UpdateNetworkPolicyOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);