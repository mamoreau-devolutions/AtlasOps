namespace AtlasOps.Features.Delivery.ReleaseCalendarMonitoring;

public sealed record UpdateReleaseCalendarMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);