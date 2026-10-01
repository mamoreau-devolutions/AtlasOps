namespace AtlasOps.Features.Mobile.MobileUpdateMonitoring;

public sealed record UpdateMobileUpdateMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);