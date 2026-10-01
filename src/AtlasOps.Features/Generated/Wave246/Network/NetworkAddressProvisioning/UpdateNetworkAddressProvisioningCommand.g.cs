namespace AtlasOps.Features.Network.NetworkAddressProvisioning;

public sealed record UpdateNetworkAddressProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);