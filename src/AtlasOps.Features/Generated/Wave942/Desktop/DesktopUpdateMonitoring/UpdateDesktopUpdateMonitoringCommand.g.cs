namespace AtlasOps.Features.Desktop.DesktopUpdateMonitoring;

public sealed record UpdateDesktopUpdateMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);