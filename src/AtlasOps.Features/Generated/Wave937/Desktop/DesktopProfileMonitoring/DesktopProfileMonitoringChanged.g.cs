namespace AtlasOps.Features.Desktop.DesktopProfileMonitoring;

public sealed record DesktopProfileMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);