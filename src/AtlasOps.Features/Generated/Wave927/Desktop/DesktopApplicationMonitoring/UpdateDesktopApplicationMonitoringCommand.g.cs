namespace AtlasOps.Features.Desktop.DesktopApplicationMonitoring;

public sealed record UpdateDesktopApplicationMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);