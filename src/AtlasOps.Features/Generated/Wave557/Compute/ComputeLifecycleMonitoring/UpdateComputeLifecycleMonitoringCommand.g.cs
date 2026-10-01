namespace AtlasOps.Features.Compute.ComputeLifecycleMonitoring;

public sealed record UpdateComputeLifecycleMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);