namespace AtlasOps.Features.Delivery.ReleaseRollbackMonitoring;

public sealed record UpdateReleaseRollbackMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);