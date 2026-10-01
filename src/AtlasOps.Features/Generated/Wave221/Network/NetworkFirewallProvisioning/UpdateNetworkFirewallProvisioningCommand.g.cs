namespace AtlasOps.Features.Network.NetworkFirewallProvisioning;

public sealed record UpdateNetworkFirewallProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);