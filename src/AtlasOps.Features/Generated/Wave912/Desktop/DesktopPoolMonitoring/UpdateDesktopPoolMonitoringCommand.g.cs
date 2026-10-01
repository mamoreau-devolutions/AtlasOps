namespace AtlasOps.Features.Desktop.DesktopPoolMonitoring;

public sealed record UpdateDesktopPoolMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);