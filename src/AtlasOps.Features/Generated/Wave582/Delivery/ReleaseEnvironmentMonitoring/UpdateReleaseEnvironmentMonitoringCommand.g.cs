namespace AtlasOps.Features.Delivery.ReleaseEnvironmentMonitoring;

public sealed record UpdateReleaseEnvironmentMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);