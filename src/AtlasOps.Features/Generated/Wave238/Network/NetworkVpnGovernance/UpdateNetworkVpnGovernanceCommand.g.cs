namespace AtlasOps.Features.Network.NetworkVpnGovernance;

public sealed record UpdateNetworkVpnGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);