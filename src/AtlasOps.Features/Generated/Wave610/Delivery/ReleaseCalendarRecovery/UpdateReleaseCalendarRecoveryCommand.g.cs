namespace AtlasOps.Features.Delivery.ReleaseCalendarRecovery;

public sealed record UpdateReleaseCalendarRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);