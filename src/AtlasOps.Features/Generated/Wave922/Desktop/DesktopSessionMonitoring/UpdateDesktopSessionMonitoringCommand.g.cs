namespace AtlasOps.Features.Desktop.DesktopSessionMonitoring;

public sealed record UpdateDesktopSessionMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);