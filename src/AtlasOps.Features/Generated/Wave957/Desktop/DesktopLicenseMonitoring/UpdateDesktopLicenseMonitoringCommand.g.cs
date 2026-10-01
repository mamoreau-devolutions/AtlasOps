namespace AtlasOps.Features.Desktop.DesktopLicenseMonitoring;

public sealed record UpdateDesktopLicenseMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);