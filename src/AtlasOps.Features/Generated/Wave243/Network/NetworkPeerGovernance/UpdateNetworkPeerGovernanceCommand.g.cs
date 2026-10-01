namespace AtlasOps.Features.Network.NetworkPeerGovernance;

public sealed record UpdateNetworkPeerGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);