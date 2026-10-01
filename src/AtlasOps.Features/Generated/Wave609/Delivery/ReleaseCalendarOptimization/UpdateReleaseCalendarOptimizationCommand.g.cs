namespace AtlasOps.Features.Delivery.ReleaseCalendarOptimization;

public sealed record UpdateReleaseCalendarOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);