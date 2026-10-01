namespace AtlasOps.Features.Compute.ComputeScaleSetMonitoring;

public sealed record UpdateComputeScaleSetMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);