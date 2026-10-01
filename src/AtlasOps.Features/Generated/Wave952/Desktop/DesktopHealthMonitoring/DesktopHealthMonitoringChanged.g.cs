namespace AtlasOps.Features.Desktop.DesktopHealthMonitoring;

public sealed record DesktopHealthMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);