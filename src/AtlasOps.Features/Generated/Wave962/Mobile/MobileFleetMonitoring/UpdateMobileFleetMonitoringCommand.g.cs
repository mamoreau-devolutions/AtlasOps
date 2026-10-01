namespace AtlasOps.Features.Mobile.MobileFleetMonitoring;

public sealed record UpdateMobileFleetMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);