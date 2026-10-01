namespace AtlasOps.Features.Network.NetworkFirewallRecovery;

public sealed record UpdateNetworkFirewallRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);