namespace AtlasOps.Features.Network.NetworkRouteRecovery;

public sealed record UpdateNetworkRouteRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);