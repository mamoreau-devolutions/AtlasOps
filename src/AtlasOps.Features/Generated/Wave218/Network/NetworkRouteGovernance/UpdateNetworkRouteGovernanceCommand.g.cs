namespace AtlasOps.Features.Network.NetworkRouteGovernance;

public sealed record UpdateNetworkRouteGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);