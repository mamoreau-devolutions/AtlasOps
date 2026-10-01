namespace AtlasOps.Features.Network.NetworkPolicyGovernance;

public sealed record UpdateNetworkPolicyGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);