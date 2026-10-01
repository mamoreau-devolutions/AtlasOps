namespace AtlasOps.Features.Mobile.MobileTelemetryMonitoring;

public sealed record UpdateMobileTelemetryMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);