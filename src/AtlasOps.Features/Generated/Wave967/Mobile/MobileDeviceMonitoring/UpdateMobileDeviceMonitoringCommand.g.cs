namespace AtlasOps.Features.Mobile.MobileDeviceMonitoring;

public sealed record UpdateMobileDeviceMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);