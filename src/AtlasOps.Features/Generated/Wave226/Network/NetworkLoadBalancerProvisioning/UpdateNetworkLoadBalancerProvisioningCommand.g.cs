namespace AtlasOps.Features.Network.NetworkLoadBalancerProvisioning;

public sealed record UpdateNetworkLoadBalancerProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);