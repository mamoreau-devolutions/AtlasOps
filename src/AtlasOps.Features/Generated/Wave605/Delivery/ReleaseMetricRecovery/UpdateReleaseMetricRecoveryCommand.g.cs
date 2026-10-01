namespace AtlasOps.Features.Delivery.ReleaseMetricRecovery;

public sealed record UpdateReleaseMetricRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);