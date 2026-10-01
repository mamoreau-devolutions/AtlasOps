namespace AtlasOps.Features.Network.NetworkAddressGovernance;

public sealed record UpdateNetworkAddressGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);