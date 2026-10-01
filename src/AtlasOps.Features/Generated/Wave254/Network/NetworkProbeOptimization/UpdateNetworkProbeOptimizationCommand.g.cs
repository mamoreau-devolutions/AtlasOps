namespace AtlasOps.Features.Network.NetworkProbeOptimization;

public sealed record UpdateNetworkProbeOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);