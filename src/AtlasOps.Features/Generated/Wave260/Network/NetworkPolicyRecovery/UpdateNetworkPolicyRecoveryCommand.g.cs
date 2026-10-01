namespace AtlasOps.Features.Network.NetworkPolicyRecovery;

public sealed record UpdateNetworkPolicyRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);