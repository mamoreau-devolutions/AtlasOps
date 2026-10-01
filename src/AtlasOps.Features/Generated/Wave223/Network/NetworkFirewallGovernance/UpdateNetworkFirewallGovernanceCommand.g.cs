namespace AtlasOps.Features.Network.NetworkFirewallGovernance;

public sealed record UpdateNetworkFirewallGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);