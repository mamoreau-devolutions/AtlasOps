namespace AtlasOps.Features.Compute.ComputeMetricMonitoring;

public sealed record UpdateComputeMetricMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);