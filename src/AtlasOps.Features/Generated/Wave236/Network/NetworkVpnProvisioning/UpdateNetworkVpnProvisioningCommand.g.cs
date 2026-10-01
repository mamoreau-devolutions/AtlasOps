namespace AtlasOps.Features.Network.NetworkVpnProvisioning;

public sealed record UpdateNetworkVpnProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);