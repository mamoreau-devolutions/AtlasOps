namespace AtlasOps.Features.Desktop.DesktopPeripheralMonitoring;

public sealed record DesktopPeripheralMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);