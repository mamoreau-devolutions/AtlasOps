namespace AtlasOps.Features.Desktop.DesktopImageMonitoring;

public sealed record UpdateDesktopImageMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);