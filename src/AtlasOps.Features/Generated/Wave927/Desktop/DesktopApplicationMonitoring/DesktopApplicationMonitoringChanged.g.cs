namespace AtlasOps.Features.Desktop.DesktopApplicationMonitoring;

public sealed record DesktopApplicationMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);