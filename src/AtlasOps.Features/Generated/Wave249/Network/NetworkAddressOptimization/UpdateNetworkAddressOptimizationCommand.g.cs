namespace AtlasOps.Features.Network.NetworkAddressOptimization;

public sealed record UpdateNetworkAddressOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);