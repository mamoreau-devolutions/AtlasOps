namespace AtlasOps.Features.Desktop.DesktopPolicyMonitoring;

public sealed record UpdateDesktopPolicyMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);