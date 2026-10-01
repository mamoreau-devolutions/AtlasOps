namespace AtlasOps.Features.Desktop.DesktopPoolMonitoring;

public sealed record DesktopPoolMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);