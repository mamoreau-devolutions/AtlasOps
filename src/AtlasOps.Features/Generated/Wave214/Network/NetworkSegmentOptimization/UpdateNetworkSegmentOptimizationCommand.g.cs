namespace AtlasOps.Features.Network.NetworkSegmentOptimization;

public sealed record UpdateNetworkSegmentOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);