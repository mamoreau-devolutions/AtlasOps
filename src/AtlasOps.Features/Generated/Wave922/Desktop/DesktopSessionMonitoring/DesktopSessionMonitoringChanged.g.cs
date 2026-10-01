namespace AtlasOps.Features.Desktop.DesktopSessionMonitoring;

public sealed record DesktopSessionMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);