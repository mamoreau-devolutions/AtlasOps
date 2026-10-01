namespace AtlasOps.Features.Cloud.GcpProjectMonitoring;

public sealed record UpdateGcpProjectMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);