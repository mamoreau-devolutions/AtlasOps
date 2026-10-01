namespace AtlasOps.Features.Delivery.ReleaseMetricGovernance;

public sealed record UpdateReleaseMetricGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);