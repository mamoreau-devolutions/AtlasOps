namespace AtlasOps.Features.Mobile.MobileProfileMonitoring;

public sealed record UpdateMobileProfileMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);