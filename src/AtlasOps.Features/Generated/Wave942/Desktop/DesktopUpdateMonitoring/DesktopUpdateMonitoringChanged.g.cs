namespace AtlasOps.Features.Desktop.DesktopUpdateMonitoring;

public sealed record DesktopUpdateMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);