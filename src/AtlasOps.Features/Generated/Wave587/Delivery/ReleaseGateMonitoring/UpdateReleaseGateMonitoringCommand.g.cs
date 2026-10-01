namespace AtlasOps.Features.Delivery.ReleaseGateMonitoring;

public sealed record UpdateReleaseGateMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);