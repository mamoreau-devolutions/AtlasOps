namespace AtlasOps.Features.Mobile.MobileApplicationMonitoring;

public sealed record UpdateMobileApplicationMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);