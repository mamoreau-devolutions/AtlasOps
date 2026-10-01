namespace AtlasOps.Features.Mobile.MobilePolicyMonitoring;

public sealed record UpdateMobilePolicyMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);