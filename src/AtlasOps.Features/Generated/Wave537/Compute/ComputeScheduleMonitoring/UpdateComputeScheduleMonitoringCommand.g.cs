namespace AtlasOps.Features.Compute.ComputeScheduleMonitoring;

public sealed record UpdateComputeScheduleMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);