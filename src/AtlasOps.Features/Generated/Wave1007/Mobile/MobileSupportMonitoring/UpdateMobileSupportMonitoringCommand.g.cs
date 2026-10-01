namespace AtlasOps.Features.Mobile.MobileSupportMonitoring;

public sealed record UpdateMobileSupportMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);