namespace AtlasOps.Features.Desktop.DesktopPeripheralMonitoring;

public sealed record UpdateDesktopPeripheralMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);