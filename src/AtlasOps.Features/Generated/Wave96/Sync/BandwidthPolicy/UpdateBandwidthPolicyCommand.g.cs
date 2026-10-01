namespace AtlasOps.Features.Sync.BandwidthPolicy;

public sealed record UpdateBandwidthPolicyCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);