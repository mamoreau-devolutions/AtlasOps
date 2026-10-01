namespace AtlasOps.Features.Network.NetworkPeerProvisioning;

public sealed record UpdateNetworkPeerProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);