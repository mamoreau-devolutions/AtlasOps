namespace AtlasOps.Features.Network.NetworkPeerRecovery;

public sealed record UpdateNetworkPeerRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);