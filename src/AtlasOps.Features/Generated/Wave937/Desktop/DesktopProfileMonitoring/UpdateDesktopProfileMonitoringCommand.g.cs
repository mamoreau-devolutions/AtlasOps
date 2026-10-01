namespace AtlasOps.Features.Desktop.DesktopProfileMonitoring;

public sealed record UpdateDesktopProfileMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);