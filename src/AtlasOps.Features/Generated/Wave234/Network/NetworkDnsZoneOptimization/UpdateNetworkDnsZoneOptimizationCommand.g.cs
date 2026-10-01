namespace AtlasOps.Features.Network.NetworkDnsZoneOptimization;

public sealed record UpdateNetworkDnsZoneOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);