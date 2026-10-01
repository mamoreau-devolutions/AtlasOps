namespace AtlasOps.Features.Delivery.ReleaseCalendarGovernance;

public sealed record UpdateReleaseCalendarGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);