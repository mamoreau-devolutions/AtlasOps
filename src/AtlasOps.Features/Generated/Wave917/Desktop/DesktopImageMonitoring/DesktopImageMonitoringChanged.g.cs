namespace AtlasOps.Features.Desktop.DesktopImageMonitoring;

public sealed record DesktopImageMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);