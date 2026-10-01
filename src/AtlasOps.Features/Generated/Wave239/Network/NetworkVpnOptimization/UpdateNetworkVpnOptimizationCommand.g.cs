namespace AtlasOps.Features.Network.NetworkVpnOptimization;

public sealed record UpdateNetworkVpnOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);