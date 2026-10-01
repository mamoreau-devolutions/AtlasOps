namespace AtlasOps.Features.Desktop.DesktopLicenseMonitoring;

public sealed record DesktopLicenseMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);