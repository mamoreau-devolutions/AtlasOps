namespace AtlasOps.Features.Desktop.DesktopHealthMonitoring;

public sealed record UpdateDesktopHealthMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);