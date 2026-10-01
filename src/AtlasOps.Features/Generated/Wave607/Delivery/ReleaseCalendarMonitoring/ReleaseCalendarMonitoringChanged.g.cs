namespace AtlasOps.Features.Delivery.ReleaseCalendarMonitoring;

public sealed record ReleaseCalendarMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);