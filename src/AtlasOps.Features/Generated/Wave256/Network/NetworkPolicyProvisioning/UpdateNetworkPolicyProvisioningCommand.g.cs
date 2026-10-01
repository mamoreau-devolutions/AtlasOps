namespace AtlasOps.Features.Network.NetworkPolicyProvisioning;

public sealed record UpdateNetworkPolicyProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);