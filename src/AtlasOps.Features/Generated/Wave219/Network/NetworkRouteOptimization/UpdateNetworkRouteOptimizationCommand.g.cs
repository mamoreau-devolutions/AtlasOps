namespace AtlasOps.Features.Network.NetworkRouteOptimization;

public sealed record UpdateNetworkRouteOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);