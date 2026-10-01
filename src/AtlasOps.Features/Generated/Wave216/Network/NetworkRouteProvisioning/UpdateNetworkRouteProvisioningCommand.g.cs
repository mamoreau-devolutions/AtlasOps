namespace AtlasOps.Features.Network.NetworkRouteProvisioning;

public sealed record UpdateNetworkRouteProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);