namespace AtlasOps.Features.Network.NetworkSegmentRecovery;

public sealed record UpdateNetworkSegmentRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);