namespace AtlasOps.Features.Network.NetworkFirewallOptimization;

public sealed record UpdateNetworkFirewallOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);