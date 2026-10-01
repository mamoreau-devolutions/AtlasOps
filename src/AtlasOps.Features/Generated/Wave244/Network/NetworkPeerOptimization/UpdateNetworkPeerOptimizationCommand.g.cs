namespace AtlasOps.Features.Network.NetworkPeerOptimization;

public sealed record UpdateNetworkPeerOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);